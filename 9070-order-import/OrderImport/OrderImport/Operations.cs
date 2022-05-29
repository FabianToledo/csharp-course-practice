using Microsoft.EntityFrameworkCore;
using OrderImport.Model;

namespace OrderImport;
public class Operations
{
    private readonly OrdersContextFactory _factory;
    public Operations(OrdersContextFactory factory)
    {
        _factory = factory;
    }

    public async Task Import(IEnumerable<Customer> customers)
    {
        using var context = _factory.CreateDbContext();

        await context.AddRangeAsync(customers);
        await context.SaveChangesAsync();
    }

    public async Task Clean()
    {
        using var context = _factory.CreateDbContext();

        /// Different ways to clear all the data: *parent* (customer) and the *related* (orders) content.
        /// 1)  *Cascade delete of tracked entities*
        ///     If the relationship of the foreign key is ON DELETE CASCADE (the default when the foreign key property (order.CustomerId))
        ///     We could fetch all the parent data (Customer), Include the related data (Orders) and the delete them using RemoveRange:
        ///     If CustomerId is nullable this will UPDATE with NULL the CustomerId in the Order entry and delete the customer
        //var customers = context.Customers.Include(c => c.Orders);
        //context.Customers.RemoveRange(customers);

        /// 2)  *Cascade delete in the database*
        ///     If the relationship of the foreign key is ON DELETE CASCADE (the default when the foreign key property (order.CustomerId))
        ///     AND WE KNOW that the table was created with the foreign key with ON DELETE CASCADE:
        ///     (e.g. in SQL Server: CONSTRAINT [FK_name] FOREIGN KEY ([CustomerId]) REFERENCES [Customer] ([Id]) ON DELETE CASCADE)
        ///     Then we can delete Customers without first loading Orders and the database will take care of deleting all the orders that were related.
        context.Customers.RemoveRange(context.Customers);
        /// Note: This would result in an exception if the foreign key constraint in the database is not configured for cascade deletes

        /// 3)  *Severing a relationship*
        ///     We also could delete all the orders related without deleting the customer
        //var customersToSever = context.Customers.Include(c => c.Orders);
        //foreach (var customer in customersToSever)
        //{
        //    // If property CustomerId is NOT nullable this will DELETE the orders (ON DELETE CASCADE)
        //    //                        is nullable this will UPDATE with NULL the CustomerId in the Order entry (ON DELETE SET NULL)
        //    customer.Orders?.Clear();
        //};
        //// Then delete the customers
        //context.Customers.RemoveRange(customersToSever);

        /// 4)  *Removing a related object*
        ///     We also could delete all the orders related without deleting the customer
        //var customers = context.Customers.Include(c => c.Orders);
        //foreach (var customer in customers)
        //{
        //    if (customer.Orders != null)
        //        context.Orders.RemoveRange(customer.Orders);
        //};
        //// Then delete the customers
        //context.Customers.RemoveRange(customers);

        // Save the changes
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> Check()
    {
        using var context = _factory.CreateDbContext();
        var excededCustomers = (await context.Customers.AsNoTracking()
            .Include(c => c.Orders)
            .ToListAsync())
            .Select(c => new { c.Name, c.CreditLimit, Sum = c.Orders?.Sum(o => o.OrderValue) ?? 0 })
            .Where(a => a.Sum > a.CreditLimit)
            .Select(a => $"{a.Name} - {a.CreditLimit} - {a.Sum}");
        return excededCustomers;
    }

    public async Task Full(IEnumerable<Customer> customers)
    {
        await Import(customers);
        await Clean();
        
    }

}
