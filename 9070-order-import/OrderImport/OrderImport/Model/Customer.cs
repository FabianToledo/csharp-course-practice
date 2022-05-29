using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace OrderImport.Model;

[Index(nameof(Name), IsUnique = true)]
public class Customer
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Precision(8, 2)]
    public decimal CreditLimit { get; set; }
    public List<Order>? Orders { get; set; }
}
