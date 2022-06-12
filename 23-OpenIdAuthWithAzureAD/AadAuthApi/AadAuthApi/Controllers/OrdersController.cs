using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Identity.Web;
using System.Security.Claims;

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
        Console.WriteLine();
        Console.WriteLine("ClaimTypes");
        // There are 2 classes of constant values that we can use to look for a specific claim
        // ClaimTypes (defined in System.Security.Claims): Defines constants for the well-known claim types that can be assigned to a subject.
        Console.WriteLine($"Name: {User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value}");
        Console.WriteLine($"NameIdentifier: {User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value}");

        Console.WriteLine();
        Console.WriteLine("ClaimConstants");
        // ClaimConstants (defined in Microsoft.Identity.Web): Constants for claim types used by microsoft
        Console.WriteLine($"Name: {User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Name)?.Value}");
        Console.WriteLine($"NameIdentifierId: {User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.NameIdentifierId)?.Value}");
        Console.WriteLine($"Object Id: {User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ObjectId)?.Value}");
        Console.WriteLine($"Scope: {User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Scope)?.Value}");
        

        return Ok(
            new[] { "Order 1", "Order 2", "Order 3" }
        );
    }


}
