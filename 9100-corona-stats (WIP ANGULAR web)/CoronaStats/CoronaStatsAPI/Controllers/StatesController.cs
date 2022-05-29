using CoronaStatsAPI.Services;
using CoronaStatsModel.Model;
using Microsoft.AspNetCore.Mvc;


namespace CoronaStatsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StatesController : ControllerBase
{
    private readonly IDataService _dataService;

    public StatesController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<ActionResult<List<State>>> Get()
    {
        return await _dataService.GetStatesDistricts();
    }

    [HttpPost("{stateId}/cases")]
    public async Task<ActionResult<List<CovidCases>>> GetStateCases(int stateId)
    {
        return await _dataService.GetStateCovidCases(stateId);
    }
}
