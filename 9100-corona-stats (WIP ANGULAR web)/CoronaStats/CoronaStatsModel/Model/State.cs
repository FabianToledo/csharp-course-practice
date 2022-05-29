using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaStatsModel.Model;
public class State //Federal States
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<District> Districts { get; set; } = new();
}
