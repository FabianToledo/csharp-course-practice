namespace TypedClients.TypedClientServices;

public class TypedClientService : ITypedClientService
{
    private readonly HttpClient _httpClient;
    public TypedClientService(HttpClient httpClient)
    {
        //We can configure the httpClient here:
        httpClient.BaseAddress = new Uri("https://api.chucknorris.io");

        _httpClient = httpClient;
    }

    public async Task<string> GetRandom()
    {
        var uri = new Uri("/jokes/random", UriKind.Relative);

        return await _httpClient.GetStringAsync(uri);
    }
}
