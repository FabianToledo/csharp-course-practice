using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelDataLayer.Model;
public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    { }
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<RoomPrice> RoomPrices => Set<RoomPrice>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Special> Specials => Set<Special>();

}

public class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
{
    public HotelDbContext CreateDbContext(string[]? args =null)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseSqlServer(config.GetConnectionString("DefaultConnection"))
            //.UseLoggerFactory(LoggerFactory.Create(builer => builer.AddConsole()))
            .Options;

        return new HotelDbContext(options);
    }
}


