using CoronaStatsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoronaStatsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportDataController : ControllerBase
{
    private readonly IImportDataService _importDataService;
    private readonly IDataService _dataService;

    public ImportDataController(IImportDataService importDataService,
                                IDataService dataService)
    {
        _importDataService = importDataService;
        _dataService = dataService;
    }
    [HttpPost]
    public async Task<ActionResult> ImportData()
    {
        int qStatesDistricts = 0;
        if (!_dataService.AnyDistricts() && !_dataService.AnyStates())
        {
            // Get the states end district data from the service
            var statesDistrictsDto = await _importDataService.GetStatesCSVAsync();

            // Import the data to the DB
            qStatesDistricts = await _dataService.AddStatesDistricts(statesDistrictsDto);
        }

        var now = DateTimeOffset.UtcNow;

        int qDeletedCovid = await _dataService.RemoveIfExistCovidCasesAsync(DateOnly.FromDateTime(now.UtcDateTime));

        var covidDto = await _importDataService.GetCovidDataCSVAsync();
        int qCovidCases = await _dataService.AddCovidCases(covidDto, now);

        return Ok($"Added {qStatesDistricts} rows in States/Districts and {qCovidCases} rows in covid cases. Deleted {qDeletedCovid} rows of date {DateOnly.FromDateTime(now.Date)} in covid cases");
    }
}
