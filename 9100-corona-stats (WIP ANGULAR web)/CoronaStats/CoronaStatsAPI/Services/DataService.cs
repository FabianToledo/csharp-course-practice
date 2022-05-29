using CoronaStatsAPI.DTO;
using CoronaStatsModel;
using CoronaStatsModel.Model;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CoronaStatsAPI.Services;

public class DataService : IDataService
{
    private readonly CoronaStatsDataContext _context;

    public DataService(CoronaStatsDataContext context)
    {
        _context = context;
    }

    public bool AnyStates()
    {
        return _context.States.Any();

    }

    public bool AnyDistricts()
    {
        return _context.Districts.Any();
    }

    public async Task<List<State>> GetStatesDistricts()
    {
        return await _context.States
            .AsNoTracking()
            .Include(s => s.Districts)
            .ToListAsync();
    }

    public async Task<List<CovidCases>> GetStateCovidCases(int stateId)
    {
        return await _context.CovidCasesTable
            .AsNoTracking()
            .Include(c => c.District) // We do not need to include the disctrict to use the where clause, we include it to return the district too.
            .Where(c => c.District.StateId == stateId)
            .ToListAsync();
    }

    public async Task<List<CovidSumDTO>> GetCasesSumGroupedByDay()
    {
        return await _context.CovidCasesTable
            .AsNoTracking()
            .GroupBy(c => c.Date)
            .Select(g => new CovidSumDTO
            {
                Date = g.Key.Date,
                CasesSum = g.Sum(c => c.Cases),
                DeathsSum = g.Sum(c => c.Deaths),
                PopulationSum = g.Sum(c => c.Population),
                SevenDaysIncidentsSum = g.Sum(c => c.SevenDayIncidents),
            })
            .ToListAsync();
    }

    public async Task<List<CovidSumDTO>> GetCasesSumGroupedByDistrictAndDay()
    {
        return await _context.CovidCasesTable
            .AsNoTracking()
            .GroupBy(c => new { c.DistrictId, c.Date })
            .Select(g => new CovidSumDTO
            {
                DistrictId = g.Key.DistrictId,
                Date = g.Key.Date.Date,
                CasesSum = g.Sum(c => c.Cases),
                DeathsSum = g.Sum(c => c.Deaths),
                PopulationSum = g.Sum(c => c.Population),
                SevenDaysIncidentsSum = g.Sum(c => c.SevenDayIncidents),
            })
            .ToListAsync();
    }

    public async Task<int> AddStatesDistricts(IEnumerable<StatesDistrictsDTO> dto)
    {
        var states = dto.DistinctBy(d => d.DistrictCode)
            .GroupBy(d => d.StateName, d => new District() { Code = d.DistrictCode, Name = d.DistrictName })
            .Select(a => new State()
            {
                Name = a.Key,
                Districts = a.ToList(),
            });
        
        _context.States.AddRange(states);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> RemoveIfExistCovidCasesAsync(DateOnly date)
    {
        var toRemove = (await _context.CovidCasesTable.ToListAsync()).Where(c => DateOnly.FromDateTime(c.Date.UtcDateTime) == date);
        _context.RemoveRange(toRemove);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> AddCovidCases(IEnumerable<CovidDataDTO> dto, DateTimeOffset date)
    {
        var districts = await _context.Districts.ToListAsync();

        var casesToAdd = dto.Select(dto => new CovidCases()
        {
            Population = dto.Population,
            Cases = dto.Cases,
            Deaths = dto.TotDeaths,
            SevenDayIncidents = dto.SevenDayCases,
            Date = date,
            DistrictId = districts.FirstOrDefault(d => d.Code == dto.DistrictCode)?.Id ?? 0,
        });

        _context.AddRange(casesToAdd);
        return await _context.SaveChangesAsync();
    }
}
