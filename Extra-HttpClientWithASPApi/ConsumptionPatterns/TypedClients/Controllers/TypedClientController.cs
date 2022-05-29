using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypedClients.TypedClientServices;

namespace TypedClients.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TypedClientController : ControllerBase
{
    private readonly ITypedClientService _typedClientService;

    public TypedClientController(ITypedClientService typedClientService)
    {
        _typedClientService = typedClientService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetRandom()
    {
        return await _typedClientService.GetRandom();
    }
}
