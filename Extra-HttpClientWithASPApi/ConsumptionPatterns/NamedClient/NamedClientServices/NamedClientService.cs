namespace NamedClient.NamedClientServices;

public class NamedClientService : INamedClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public NamedClientService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetRandom()
    {
        
        var uri = new Uri("/jokes/random", UriKind.Relative);

        var httpClient = _httpClientFactory.CreateClient("NamedClient");

        return await httpClient.GetStringAsync(uri);
    }
}
