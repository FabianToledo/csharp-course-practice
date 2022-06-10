using AnimalCountingDatabase.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnimalCountingDatabase.Tests;
public class DemoTests
{
    [Fact]
    public void Test1()
    {
        Assert.True(true);
        Assert.False(false);
    }

    [Fact]
    public async Task CustomerIntegrationTest()
    {
        // Create a DB context
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<CustomerContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;

        var context = new CustomerContext(options);

        // We can ensure that we start from scratch
        await context.Database.EnsureDeletedAsync();
        // Create the test database and tables
        await context.Database.EnsureCreatedAsync();

        // Delete all existing customers in the db
        // As we added the EnsureDeleted and EnsureCreated we do not need to remove the customers anymore
        // context.Customers.RemoveRange(await context.Customers.ToArrayAsync());
        // await context.SaveChangesAsync();

        // Create a controller
        var controller = new CustomersController(context);

        // Add a customer
        await controller.Add(new Customer() { CustomerName = "FooBar" });

        // Check: Does GetAll return the added customer?
        var customers = await controller.Get();

        Assert.Single(customers);
        Assert.Equal("FooBar", customers.FirstOrDefault()?.CustomerName);
    }

}