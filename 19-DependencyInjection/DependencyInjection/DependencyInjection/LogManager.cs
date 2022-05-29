namespace DependencyInjection;

public class LogManager
{
    private readonly ProductContext context;

    public LogManager(ProductContext context)
    {
        this.context = context;
    }

    public void AddLog(string user, string description)
    {
        context.Logs.Add(
            new Log
            {
                User = user,
                Description = description,
                LogDateTime = DateTime.UtcNow,
            });
    }
}
