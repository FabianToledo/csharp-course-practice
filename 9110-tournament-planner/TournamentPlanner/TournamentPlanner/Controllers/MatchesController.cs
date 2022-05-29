using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TournamentPlanner.Data;

namespace TournamentPlanner.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MatchesController : ControllerBase
{
    private readonly TournamentPlannerDbContext _context;

    public MatchesController(TournamentPlannerDbContext context)
    {
        _context = context;
    }

    [HttpGet("open")]
    public async Task<IList<Match>> GetIncompleteMatches()
    {
        return await _context.GetIncompleteMatches();
    }

    [HttpPost("generate")]
    public async Task GenerateMatchesForNextRound()
    {
        await _context.GenerateMatchesForNextRound();
    }

}
