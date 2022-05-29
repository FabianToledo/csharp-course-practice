namespace BasicUsage.BasicUsageServices;

public class BasicUsageService : IBasicUsageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public BasicUsageService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetRandom()
    {
        var baseUri = new Uri("https://api.chucknorris.io");
        var uri = new Uri(baseUri, "/jokes/random");

        var httpClient = _httpClientFactory.CreateClient();

        return await httpClient.GetStringAsync(uri);
    }
}
