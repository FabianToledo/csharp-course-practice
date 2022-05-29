using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentPlanner.Data
{
    public class Match
    {
        public int ID { get; set; }
        [Range(1, 5, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Round { get; set; }
        
        //[InverseProperty(nameof(Player.MatchesAsPlayer1))]
        public Player Player1 { get; set; } = null!;
        public int Player1ID { get; set; }
        
        //[InverseProperty(nameof(Player.MatchesAsPlayer2))]
        public Player Player2 { get; set; } = null!;
        public int Player2ID { get; set; }

        //[InverseProperty(nameof(Player.MatchesWon))]
        public Player? Winner { get; set; } = null;
        public int? WinnerID { get; set; }

    }
}
