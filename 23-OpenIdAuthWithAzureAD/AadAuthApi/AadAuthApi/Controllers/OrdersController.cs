using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace AadAuthApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllOrders()
    {
        // We can get the token claims to decide which type of authorization the user has.
        Console.WriteLine("Bearer token data:");
        Console.WriteLine($"User: {User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Name)}");
        Console.WriteLine($"{User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Scope)}");

        return Ok(
            new[] { "Order 1", "Order 2", "Order 3" }
        );
    }


}
