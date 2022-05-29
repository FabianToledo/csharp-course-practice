using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TournamentPlanner.Data;

namespace TournamentPlanner.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly TournamentPlannerDbContext _context;

    public PlayersController(TournamentPlannerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IList<Player>> GetAllPlayers(string? name)
    {
        return await _context.GetFilteredPlayers(name);
    }

    [HttpPost]
    public async Task<Player> AddPlayer(Player player)
    {
        return await _context.AddPlayer(player);
    }
}
