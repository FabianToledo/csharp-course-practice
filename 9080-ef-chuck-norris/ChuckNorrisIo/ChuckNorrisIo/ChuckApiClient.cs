

using ChuckNorrisIo.DTO;
using ChuckNorrisIo.Model;
using System.Text.Json;

namespace ChuckNorrisIo;
public class ChuckApiClient : IDisposable
{
    HttpClient? client = new();
    readonly string baseUri = $"https://api.chucknorris.io/jokes";
   
    public async Task<IEnumerable<string>> GetCategories()
    {
        if (client == null) return new List<string>();
        var categoriesJson = await client.GetStringAsync($"{baseUri}/categories");

        var categories = JsonSerializer.Deserialize<IEnumerable<string>>(categoriesJson);

        return categories ?? new List<string>();
    }

    public IEnumerable<string> FilterCategories(IEnumerable<string> categories, params string[] filterCategories)
    {
        return categories.Where(c => !filterCategories.Contains(c));
    }

    public async Task<ChuckWitz> GetRandomJoke(IEnumerable<string> categories)
    {
        if (client == null) return new ChuckWitz();

        var jokeJson = await client.GetStringAsync($"{baseUri}/random?category={string.Join(",", categories)}");
        
        var joke = JsonSerializer.Deserialize<JokeDto>(jokeJson);

        if (joke == null) return new ChuckWitz(); 
        
        return DtoToModel(joke);
        
    }

    private static ChuckWitz DtoToModel(JokeDto dto)
    {
        return new ChuckWitz()
        {
            ChuckNorrisId = dto.id ?? "",
            Url = dto.url ?? "",
            Witz = dto.value ?? ""
        };
    }

    public async IAsyncEnumerable<ChuckWitz> GetJokes(IEnumerable<string> categories, int qJokes)
    {
        var tasks = new List<Task<ChuckWitz>>();
        for (int i = 0; i < qJokes; i++)
        {
            tasks.Add(GetRandomJoke(categories));
        }
        foreach(var task in tasks)
        {
            yield return await task;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }
    }
}
