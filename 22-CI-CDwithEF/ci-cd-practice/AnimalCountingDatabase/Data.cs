using Microsoft.EntityFrameworkCore;

namespace AnimalCountingDatabase;


public class Customer
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}

public class CustomerContext: DbContext
{
    public CustomerContext(DbContextOptions<CustomerContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }

}