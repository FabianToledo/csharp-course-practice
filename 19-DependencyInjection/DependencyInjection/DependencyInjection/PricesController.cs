using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DependencyInjection;

public record PriceChangeDto(string User, decimal PercPriceChange);

[Route("api/[controller]")]
[ApiController]
public class PricesController : ControllerBase
{
    private readonly ProductContext context;
    private readonly PriceManager pm;
    private readonly LogManager lm;

    public PricesController(ProductContext context,
                            PriceManager pm,
                            LogManager lm)
    {
        this.context = context;
        this.pm = pm;
        this.lm = lm;
    }

    [HttpPost("fill")]
    public async Task<IActionResult> FillWithDemoData()
    {
        context.Prices.Add(
            new Price()
            {
                Product = "Apples",
                ProductPrice = 100
            });
        await context.SaveChangesAsync();
        return StatusCode((int)HttpStatusCode.Created);
    }

    [HttpPost]
    public async Task<IActionResult> PriceChange(PriceChangeDto priceChange)
    {
        pm.ChangePrices(priceChange.PercPriceChange);
        lm.AddLog(priceChange.User, $"Price chenged by {priceChange.PercPriceChange * 100}");
        await context.SaveChangesAsync();
        return Ok();
    }



}
