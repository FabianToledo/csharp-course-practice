using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BasicUsage.BasicUsageServices;

namespace BasicUsage.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BasicUsageController : ControllerBase
{
    private readonly IBasicUsageService _basicUsageService;

    public BasicUsageController(IBasicUsageService basicUsageService)
    {
        _basicUsageService = basicUsageService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetRandom()
    {
        return await _basicUsageService.GetRandom();
    }
}
