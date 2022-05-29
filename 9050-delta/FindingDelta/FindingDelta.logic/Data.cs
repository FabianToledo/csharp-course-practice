using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindingDelta.logic;
public class Data
{
    public string book_iban { get; set; } = string.Empty;
    public string book_title { get; set; } = string.Empty;
    public string genre { get; set; } = string.Empty;
    public int year { get; set; }
    public int revenue { get; set; }

    public override string ToString()
    {
        return $"{book_iban}\t{book_title}\t{genre}\t{year}\t{revenue}";
    }
}
