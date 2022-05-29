using CoronaStatsAPI.DTO;
using CoronaStatsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoronaStatsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CasesController : ControllerBase
{
    private readonly IDataService _dataService;

    public CasesController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CovidSumDTO>>> GetCases()
    {
        return await _dataService.GetCasesSumGroupedByDay();
    }

    [HttpGet("byDistrict")]
    public async Task<ActionResult<List<CovidSumDTO>>> GetCasesByDistrict()
    {
        return await _dataService.GetCasesSumGroupedByDistrictAndDay();
    }
    
}
