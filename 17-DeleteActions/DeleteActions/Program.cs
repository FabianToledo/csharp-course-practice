using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

var factory = new DemoContextFactory();
using var context = factory.CreateDbContext();

Console.WriteLine();

var header = new OrderHeader()
{
    Description = "Foo Bar"
};
var detail1 = new OrderDetail()
{
    Product = "Foo",
    Quantity = 1,
    OrderHeader = header,
};
var detail2 = new OrderDetail()
{
    Product = "Bar",
    Quantity = 2,
    OrderHeader = header,
};

context.OrderDetails.Add(detail1);
context.OrderDetails.Add(detail2);
await context.SaveChangesAsync();

context.OrderHeaders.Remove(header);
await context.SaveChangesAsync();


public class OrderHeader
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    [JsonIgnore]
    public List<OrderDetail>? OrderDetails { get; set; }
}

public class OrderDetail
{
    public int Id { get; set; }
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public OrderHeader? OrderHeader { get; set; }
    public int? OrderHeaderId { get; set; }

}

public class DemoContext : DbContext
{
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DemoContext(DbContextOptions<DemoContext> options) : base(options) { }

    // We can configure the delete behaviors of our database.
    // By default:  a not nullable foreign Id relationship will Cascade deletions.
    //                  CONSTRAINT [FK_OrderDetails_OrderHeaders_OrderHeaderId] FOREIGN KEY ([OrderHeaderId]) REFERENCES [OrderHeaders] ([Id]) ON DELETE CASCADE
    //              a nullable foreign Id relationship will update the foreign key to null
    //                  CONSTRAINT [FK_OrderDetails_OrderHeaders_OrderHeaderId] FOREIGN KEY ([OrderHeaderId]) REFERENCES [OrderHeaders] ([Id])
    // Another way to configure the behavior is overriding the OnModelCreating method and using the Fluent API
    //              
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Example to configure a different approach to deletions
        modelBuilder.Entity<OrderHeader>()
            .HasMany(h => h.OrderDetails)
            .WithOne(d => d.OrderHeader)
            .HasForeignKey(d => d.OrderHeaderId)
            .OnDelete(DeleteBehavior.NoAction)
            //.OnDelete(DeleteBehavior.SetNull)
            ;

    }
}
public class DemoContextFactory : IDesignTimeDbContextFactory<DemoContext>
{
    public DemoContext CreateDbContext(string[]? args = null)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<DemoContext>()
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;

        return new DemoContext(options);
    }
}