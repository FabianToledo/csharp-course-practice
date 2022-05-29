using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesStatistics.Entities;
public class Order
{
    public int OrderId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string DeliverToCountry { get; set; } = string.Empty;
    public List<Detail> Details { get; set; } = new List<Detail>();

    static public Order Deserialize(string orderString)
    {
        var fields = orderString.Trim().Split('\t');
        if (fields.Length != 4) throw new ArgumentOutOfRangeException(nameof(fields.Length), "Has to be equal to 4");
        return new Order()
        {
            OrderId = int.Parse(fields[1]),
            Customer = fields[2],
            DeliverToCountry = fields[3],
        };
    }
}
