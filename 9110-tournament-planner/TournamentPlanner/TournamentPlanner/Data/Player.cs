using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentPlanner.Data
{
    public class Player
    {
        public int ID { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? Phone { get; set; }

        //[InverseProperty(nameof(Match.Player1))]
        public List<Match> MatchesAsPlayer1 { get; set; } = new();

        //[InverseProperty(nameof(Match.Player2))]
        public List<Match> MatchesAsPlayer2 { get; set; } = new();

        //[InverseProperty(nameof(Match.Winner))] 
        public List<Match> MatchesWon { get; set; } = new();
    }
}
