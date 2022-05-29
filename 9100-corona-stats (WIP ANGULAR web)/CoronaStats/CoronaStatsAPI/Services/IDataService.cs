using CoronaStatsAPI.DTO;
using CoronaStatsModel.Model;

namespace CoronaStatsAPI.Services;

public interface IDataService
{
    public bool AnyStates();
    public bool AnyDistricts();
    Task<List<State>> GetStatesDistricts();
    Task<List<CovidCases>> GetStateCovidCases(int stateId);
    Task<List<CovidSumDTO>> GetCasesSumGroupedByDay();
    Task<List<CovidSumDTO>> GetCasesSumGroupedByDistrictAndDay();
    Task<int> AddStatesDistricts(IEnumerable<StatesDistrictsDTO> dto);
    Task<int> RemoveIfExistCovidCasesAsync(DateOnly date);
    Task<int> AddCovidCases(IEnumerable<CovidDataDTO> dto, DateTimeOffset date);

}
