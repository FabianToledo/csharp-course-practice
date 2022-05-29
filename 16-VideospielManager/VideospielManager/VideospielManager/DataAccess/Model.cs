using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VideospielManager.DataAccess
{
    public class GameGenre
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Game>? Games { get; set; }

    }

    public class Game
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        public GameGenre? Genre { get; set; }
        public int? GenreId { get; set; }
        public int PersonalRating { get; set; }
    }

}
