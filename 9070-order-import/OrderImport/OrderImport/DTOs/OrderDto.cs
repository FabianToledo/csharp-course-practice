using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderImport.DTOs;
public class OrderDto
{
    public string CustomerName { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public decimal OrderValue { get; set; }

}
