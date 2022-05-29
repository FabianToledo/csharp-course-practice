using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChuckNorrisIo.Model;
public class ChuckWitzContext : DbContext
{
    public ChuckWitzContext(DbContextOptions<ChuckWitzContext> options) : base(options)
    { }

    public DbSet<ChuckWitz> ChuckWitze {  get; set; }

}

public class ChuckWitzContextFactory : IDesignTimeDbContextFactory<ChuckWitzContext>
{
    public ChuckWitzContext CreateDbContext(string[]? args = null)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<ChuckWitzContext>()
            .UseSqlServer(config.GetConnectionString("DefaultConnection"))
            .UseLoggerFactory(LoggerFactory.Create(builer => builer.AddConsole()))
            .Options;

        return new ChuckWitzContext(options);
    }
}
