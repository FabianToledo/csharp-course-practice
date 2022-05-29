namespace DependencyInjection;

public class PriceManager
{
    private readonly ProductContext context;

    public PriceManager(ProductContext context)
    {
        this.context = context;
    }

    public void ChangePrices(decimal percPriceChange)
    {
        foreach (var p in context.Prices)
        {
            p.ProductPrice *= percPriceChange;
        }
    }
}
