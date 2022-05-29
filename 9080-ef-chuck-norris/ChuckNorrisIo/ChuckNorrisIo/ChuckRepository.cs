

using ChuckNorrisIo.Model;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisIo;
public class ChuckRepository
{
    private readonly ChuckWitzContextFactory _factory;
    private readonly ChuckWitzContext _context;


    public ChuckRepository(ChuckWitzContextFactory factory)
    {
        _factory = factory;
        _context = factory.CreateDbContext();
    }

    public async Task Save(ChuckWitz joke)
    {
        //using var context = _factory.CreateDbContext();

        _context.ChuckWitze.Add(joke);
        await _context.SaveChangesAsync();
    }

    public async Task SaveRange(IEnumerable<ChuckWitz> jokes)
    {
        //using var context = _factory.CreateDbContext();

        _context.ChuckWitze.AddRange(jokes);
        await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SaveRange Error. Rolling back transaction. Exception message: {ex.Message}");
            await _context.Database.RollbackTransactionAsync();
        }
    }

    public async Task<IEnumerable<ChuckWitz>> GetAll()
    {
        //using var context = _factory.CreateDbContext();

        return await _context.ChuckWitze.ToListAsync();
    }

    public IAsyncEnumerable<ChuckWitz> GetAllAsyncEnum()
    {
        //using var context = _factory.CreateDbContext();

        return _context.ChuckWitze.AsAsyncEnumerable();
    }

    public async Task<bool> JokeExists(ChuckWitz joke)
    {
        //using var context = _factory.CreateDbContext();

        return await _context.ChuckWitze.AnyAsync(j => j.ChuckNorrisId == joke.ChuckNorrisId);
    }

    public async Task<int> ClearAll()
    {
        return await _context.Database.ExecuteSqlRawAsync("DELETE FROM ChuckWitze");
    }

}
