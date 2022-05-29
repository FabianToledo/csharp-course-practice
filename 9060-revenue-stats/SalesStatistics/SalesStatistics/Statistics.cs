using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesStatistics.Entities;

namespace SalesStatistics;
public class Statistics
{

    public static IEnumerable<string> SummarizeByOrderID(IEnumerable<Order> orders)
    {
        return orders
            .Select(o => new
            {
                o.OrderId,
                Revenue = o.Details.Sum(d => d.PriceEur)
            })
            .OrderByDescending(a => a.Revenue)
            .Select(a => $"{a.OrderId}: {a.Revenue}");
    }

    public static IEnumerable<string> SummarizeByCustomer(IEnumerable<Order> orders, bool displayPerc = false)
    {
        var customerRev = orders
            .GroupBy(o => o.Customer, o => o.Details.Sum(d => d.PriceEur))
            .Select(g => new
            {
                Customer = g.Key,
                Revenue = g.Sum(),
            });

        var totalRev = customerRev.Sum(cr => cr.Revenue);

        if (displayPerc)
            return customerRev.Select(a => $"{a.Customer}: {a.Revenue} ({100*a.Revenue/totalRev:F} %)");
        else
            return customerRev.Select(a => $"{a.Customer}: {a.Revenue}");

    }
}
