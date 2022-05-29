using Microsoft.EntityFrameworkCore;

namespace OrderImport.Model;
public class Order
{
    public int Id { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    [Precision(8, 2)]
    public decimal OrderValue { get; set; }
    public Customer? Customer { get; set; }
    public int CustomerId { get; set; }
}
