using Microsoft.EntityFrameworkCore;
using HotelDataLayer.Model;
using System.Text.Json;

var factory = new HotelDbContextFactory();
using var context = factory.CreateDbContext();

var hotels = context.Hotels.AsNoTracking()
    .Include(h => h.Specials)
    .Include(h => h.Address)
    .Include(h => h.RoomTypes)
    .ThenInclude(rt => rt.RoomPrice)
    ;

var mdHotels = hotels.Select(h =>
@$"# {h.Name}

## Location

{h.Address.Street}
{h.Address.ZipCode} {h.Address.City}

## Specials

{string.Join("\n", h.Specials.Select(s => $"* {s.Name}"))}

## Room Types
| Room Type   |  Size | Price Valid From | Price Valid To | Price in € |
| ----------- | ----: | ---------------- | -------------- | ---------: |
{string.Join("\n", h.RoomTypes.Select(rt => 
$"| {rt.Title} | {rt.Size} | {rt.RoomPrice.DateFrom} | {rt.RoomPrice.DateTo} | {rt.RoomPrice.Price} |"))}

");

using var file = File.CreateText("export.md");

await foreach (var mdH in mdHotels.AsAsyncEnumerable())
{
    //Console.WriteLine(mdH);
    await file.WriteAsync(mdH);
}

