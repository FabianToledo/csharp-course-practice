using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesStatistics.Entities;
public class Detail
{
    public string Product { get; set; } = string.Empty;
    public int Amount { get; set; }
    public decimal UnitPriceEur { get; set; }
    public decimal PriceEur { get; set; }
    public Order? Order { get; set; }

    static public Detail Deserialize(string detailString)
    {
        var fields = detailString.Trim().Split('\t');
        if (fields.Length != 5) throw new ArgumentOutOfRangeException(nameof(fields.Length), "Has to be equal to 5");
        return new Detail()
        {
            Product = fields[1],
            Amount = int.Parse(fields[2]),
            UnitPriceEur = decimal.Parse(fields[3]),
            PriceEur = decimal.Parse(fields[4]),
        };
    }

}
