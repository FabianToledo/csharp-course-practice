using Microsoft.EntityFrameworkCore;

namespace DependencyInjection;

public class Price
{
    public int Id { get; set; }
    public string Product { get; set; } = string.Empty;
    [Precision(10,2)]
    public decimal ProductPrice { get; set; }
}
public class Log
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public DateTime LogDateTime { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

    public DbSet<Price> Prices => Set<Price>();
    public DbSet<Log> Logs => Set<Log>();

}

