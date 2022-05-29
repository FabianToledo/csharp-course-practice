using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentPlanner.Data
{
    public enum PlayerNumber { Player1 = 1, Player2 = 2 };

    public class TournamentPlannerDbContext : DbContext
    {
        public TournamentPlannerDbContext(DbContextOptions<TournamentPlannerDbContext> options)
            : base(options)
        { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Player1)
                .WithMany(p => p.MatchesAsPlayer1)
                //.WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                ;

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Player2)
                .WithMany(p => p.MatchesAsPlayer2)
                //.WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                ;

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Winner)
                .WithMany(p => p!.MatchesWon)
                //.WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                ;
        }

        // This class is NOT COMPLETE.
        // Todo: Complete the class according to the requirements

        /// <summary>
        /// Adds a new player to the player table
        /// </summary>
        /// <param name="newPlayer">Player to add</param>
        /// <returns>Player after it has been added to the DB</returns>
        public async Task<Player> AddPlayer(Player newPlayer)
        {
            Players.Add(newPlayer);
            await SaveChangesAsync();
            return newPlayer;
        }

        /// <summary>
        /// Adds a match between two players
        /// </summary>
        /// <param name="player1Id">ID of player 1</param>
        /// <param name="player2Id">ID of player 2</param>
        /// <param name="round">Number of the round</param>
        /// <returns>Generated match after it has been added to the DB</returns>
        public async Task<Match> AddMatch(int player1Id, int player2Id, int round)
        {
            var match = new Match()
            {
                Player1ID = player1Id,
                Player2ID = player2Id,
                Round = round,
            };
            Matches.Add(match);
            await SaveChangesAsync();
            return match;
        }

        /// <summary>
        /// Set winner of an existing game
        /// </summary>
        /// <param name="matchId">ID of the match to update</param>
        /// <param name="player">Player who has won the match</param>
        /// <returns>Match after it has been updated in the DB</returns>
        public async Task<Match> SetWinner(int matchId, PlayerNumber player)
        {
            var match = await Matches.FirstOrDefaultAsync(m => m.ID == matchId);
            if(match == null) throw new InvalidOperationException("Invalid match ID");

            match.WinnerID = player switch
            {
                PlayerNumber.Player1 => match.Player1ID,
                PlayerNumber.Player2 => match.Player2ID,
                _ => null
            };
            await SaveChangesAsync();
            return match;
        }

        /// <summary>
        /// Get a list of all matches that do not have a winner yet
        /// </summary>
        /// <returns>List of all found matches</returns>
        public async Task<IList<Match>> GetIncompleteMatches()
        {
            return await Matches.Where(m => m.WinnerID == null).ToListAsync();
        }

        /// <summary>
        /// Delete everything (matches, players)
        /// </summary>
        public async Task DeleteEverything()
        {
            try
            {
                await Database.BeginTransactionAsync();
                Matches.RemoveRange(await Matches.ToListAsync());
                Players.RemoveRange(await Players.ToListAsync());
                await SaveChangesAsync();
                await Database.CommitTransactionAsync();
            }
            catch
            {
                await Database.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Get a list of all players whose name contains <paramref name="playerFilter"/>
        /// </summary>
        /// <param name="playerFilter">Player filter. If null, all players must be returned</param>
        /// <returns>List of all found players</returns>
        public async Task<IList<Player>> GetFilteredPlayers(string? playerFilter = null)
        {
            var players = playerFilter == null ? Players : Players.Where(p => p.Name.Contains(playerFilter));
            return await players.ToListAsync();
        }

        /// <summary>
        /// Generate match records for the next round
        /// </summary>
        /// <exception cref="InvalidOperationException">Error while generating match records</exception>
        public async Task GenerateMatchesForNextRound()
        {
            try
            {
                await Database.BeginTransactionAsync();
                if (await Matches.AsNoTracking().AnyAsync(m => m.WinnerID == null))
                    throw new InvalidOperationException("There are at least one match without a winner");

                var allPlayersID = await Players.AsNoTracking().Select(p => p.ID).ToListAsync();

                int qPlayers = allPlayersID.Count;
                if (qPlayers != 32)
                    throw new InvalidOperationException($"The number of players in the DB is {qPlayers}. Should be 32.");

                var prevMatchesWinners = await Matches
                    .AsNoTracking()
                    .ToListAsync();
                var qPrevMatchesFinished = prevMatchesWinners.Count;

                var nextMatches = qPrevMatchesFinished switch
                {
                    0 => allPlayersID.CreateMatches(1),
                    16 => prevMatchesWinners.Where(m => m.Round == 1).Select(m => m.WinnerID!.Value).CreateMatches(2),
                    24 => prevMatchesWinners.Where(m => m.Round == 2).Select(m => m.WinnerID!.Value).CreateMatches(3),
                    28 => prevMatchesWinners.Where(m => m.Round == 3).Select(m => m.WinnerID!.Value).CreateMatches(4),
                    30 => prevMatchesWinners.Where(m => m.Round == 4).Select(m => m.WinnerID!.Value).CreateMatches(5),
                    _ => throw new InvalidOperationException($"The number of matches {qPrevMatchesFinished} is not the expected.")
                };

                await Matches.AddRangeAsync(nextMatches);
                await SaveChangesAsync();
                await Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await Database.RollbackTransactionAsync();
                throw new InvalidOperationException($"Rolling Back: {ex.Message}");
            }
        }
    }

    public static class TournamentPlannerExtensions
    {
        public static IEnumerable<Match> CreateMatches(this IEnumerable<int> playersID, int round)
        {
            int i = 0;
            Match currentMatch = new();
            var rand = new Random();

            foreach (var playerID in playersID.OrderBy(p => rand.Next()))
            {
                if (i++ % 2 == 0)
                {
                    currentMatch = new Match()
                    {
                        Round = round,
                        Player1ID = playerID,
                    };
                }
                else
                {
                    currentMatch.Player2ID = playerID;
                    yield return currentMatch;
                }
            };
        }
    }

}
