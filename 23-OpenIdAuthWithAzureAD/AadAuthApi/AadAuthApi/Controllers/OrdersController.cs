using Microsoft.AspNetCore.Mvc;

namespace AadAuthApi.Controllers;
[Route("api/[controller]")]
[ApiController]
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
