using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaStatsModel.Model;
public class District // District
{
    public int Id { get; set; }
    public State State { get; set; } = null!;
    public int StateId { get; set; }
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<CovidCases> CovidCasesList { get; set; } = new();
}
