using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalCountingDatabase.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly CustomerContext context;

    public CustomersController(CustomerContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Customer>> Get() 
        => await context.Customers.ToArrayAsync();
    
    [HttpPost]
    public async Task<Customer> Add([FromBody] Customer c)
    {
        context.Customers.Add(c);
        await context.SaveChangesAsync();
        return c;
    }
}
