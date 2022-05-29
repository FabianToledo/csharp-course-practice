using Microsoft.AspNetCore.Mvc;
using VideospielManager.DataAccess;

namespace VideospielManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly VideoGameDataContext _context;
        public GamesController(VideoGameDataContext videoGameDataContext)
        {
            _context = videoGameDataContext;
        }

        [HttpGet]
        public IEnumerable<Game> GetAllGames() => _context.Games;

        [HttpGet]
        [Route("{id}")]
        public Game? GetGameById(int id) => _context.Games.FirstOrDefault(g => g.Id == id);

        [HttpGet]
        [Route("[action]")]
        public Game? Query([FromQuery]int id) => _context.Games.FirstOrDefault(g => g.Id == id);

        [HttpPost]
        public async Task<Game> AddGame([FromBody] Game game)
        {
            _context.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

    }
}
