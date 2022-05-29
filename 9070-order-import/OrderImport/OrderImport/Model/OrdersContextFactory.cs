using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OrderImport.Model;
public class OrdersContextFactory : IDesignTimeDbContextFactory<OrdersContext>
{
    public OrdersContext CreateDbContext(string[]? args = null)
    {
        var appSettingsFileName = "appsettings.json";
        var config = new ConfigurationBuilder().AddJsonFile(appSettingsFileName).Build();

        var options = new DbContextOptionsBuilder<OrdersContext>()
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(config.GetConnectionString("DefaultConnection"))
            .Options;

        return new OrdersContext(options);
    }
}
