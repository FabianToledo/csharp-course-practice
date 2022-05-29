using Microsoft.EntityFrameworkCore;

namespace VideospielManager.DataAccess
{
    public class VideoGameDataContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<GameGenre> Genres { get; set; }

        public VideoGameDataContext(DbContextOptions<VideoGameDataContext> options) : base(options)
        { }


    }
}
