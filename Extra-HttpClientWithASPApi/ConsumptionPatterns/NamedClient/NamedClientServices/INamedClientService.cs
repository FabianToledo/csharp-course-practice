namespace NamedClient.NamedClientServices;

public interface INamedClientService
{
    public Task<string> GetRandom();
}
