using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaStatsModel.Model;
public class CovidCases
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public District District { get; set; } = null!;
    public int DistrictId { get; set; }
    public int Population { get; set; }
    public int Cases { get; set; }
    public int Deaths { get; set; }
    public int SevenDayIncidents { get; set; }

}
