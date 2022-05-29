namespace TypedClients.TypedClientServices;

public interface ITypedClientService
{
    public Task<string> GetRandom();
}
