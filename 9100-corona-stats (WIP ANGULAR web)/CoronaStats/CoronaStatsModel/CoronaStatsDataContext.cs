using CoronaStatsModel.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoronaStatsModel;
public class CoronaStatsDataContext : DbContext
{
    public CoronaStatsDataContext(DbContextOptions<CoronaStatsDataContext> options) : base(options)
    { }

    public DbSet<State> States => Set<State>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<CovidCases> CovidCasesTable => Set<CovidCases>();
}
