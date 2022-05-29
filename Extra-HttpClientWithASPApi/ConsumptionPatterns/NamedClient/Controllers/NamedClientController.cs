using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NamedClient.NamedClientServices;

namespace NamedClient.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NamedClientController : ControllerBase
{
    private readonly INamedClientService _namedClientService;

    public NamedClientController(INamedClientService namedClientService)
    {
        _namedClientService = namedClientService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetRandom()
    {
        return await _namedClientService.GetRandom();
    }
}
