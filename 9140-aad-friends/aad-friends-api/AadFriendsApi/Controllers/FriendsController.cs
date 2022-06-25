using AadFriendsApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AadFriendsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly FriendsContext context;

    public FriendsController(FriendsContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Friend>> GetAll()
    {
        return await context.Friends.ToListAsync();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Friend friend)
    {
        var friends = context.Friends.FirstOrDefault(f => f.AadId.Equals(friend.AadId));
        if (friends == null)
        {
            context.Friends.Add(friend);
            await context.SaveChangesAsync();
            return Created("", friend);
        }
        return NoContent();
    }

    [HttpDelete("{aadId}")]
    public async Task<IActionResult> Delete([FromRoute] string aadId)
    {
        var friends = context.Friends.FirstOrDefault(f => f.AadId.Equals(aadId));
        if (friends != null)
        {
            context.Friends.Remove(friends);
            await context.SaveChangesAsync();
            return NoContent();
        }
        return NotFound();
    }

}
