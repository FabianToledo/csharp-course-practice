using CoronaStatsAPI.DTO;

namespace CoronaStatsAPI.Services;

public interface IImportDataService
{
    Task<List<StatesDistrictsDTO>> GetStatesCSVAsync();
    Task<List<CovidDataDTO>> GetCovidDataCSVAsync();
}
