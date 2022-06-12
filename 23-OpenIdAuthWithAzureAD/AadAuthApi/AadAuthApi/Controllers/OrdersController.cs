using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AadAuthApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllOrders()
    {
        return Ok(
            new[] { "Order 1", "Order 2", "Order 3" }
        );
    }


}
