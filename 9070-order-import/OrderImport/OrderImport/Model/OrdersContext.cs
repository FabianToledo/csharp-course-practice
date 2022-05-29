using Microsoft.EntityFrameworkCore;

namespace OrderImport.Model;
public class OrdersContext : DbContext
{
    public OrdersContext(DbContextOptions<OrdersContext> options) : base(options) { }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

}
