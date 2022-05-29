using HotelDataLayer.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

Console.WriteLine("Add Hotels to the Database:");
Console.WriteLine("Leave the field empty if you want to end a list");
Console.WriteLine();
var hotels = GetRepeatingData("Name of the Hotel").Select(name =>
    new Hotel()
    {
        Name = name,
        Address = new Address()
        {
            Street = GetData("Address street"),
            ZipCode = GetData("Zip Code"),
            City = GetData("City")
        },
        Specials = GetRepeatingData("Special").Select(s => new Special() { Name = s }).ToList(),
        RoomTypes = GetRepeatingData("Room").Select(rt =>
            new RoomType()
            {
                Title = rt,
                Description = GetData("Description"),
                Size = GetData("Size"),
                RoomPrice = new RoomPrice() { Price = GetPriceData("Price (euros)") },
                IsAccessible = GetYesNoData("Is accessible? (Y/N)"),
                QuantityAvailable = GetNumericData("Quantity Available")
            }
        ).ToList(),
    })
    .Where(h => GetYesNoData("Is the data correct?"))
    ;

var factory = new HotelDbContextFactory();
using var context = factory.CreateDbContext();

await context.AddRangeAsync(hotels);
await context.SaveChangesAsync();



bool GetYesNoData(string request)
{
    string? line;
    do
    {
        Console.Write($"{request}: ");
    }
    while ( string.IsNullOrWhiteSpace(line = Console.ReadLine()?.ToUpper()) || 
           (line != "Y" && line != "N" ) );
    return line == "Y";
}

int GetNumericData(string request)
{
    int number;
    string? line;
    do
    {
        Console.Write($"{request}: ");
    }
    while (string.IsNullOrWhiteSpace(line = Console.ReadLine()) || !int.TryParse(line, out number));
    return number;
}

decimal GetPriceData(string request)
{
    decimal number; ;
    string? line;
    do
    {
        Console.Write($"{request}: ");
    }
    while (string.IsNullOrWhiteSpace(line = Console.ReadLine()) || !decimal.TryParse(line, out number));
    return number;
}


string GetData(string request)
{
    string? line;
    do
    {
        Console.Write($"{request}: ");
    }
    while (string.IsNullOrWhiteSpace(line = Console.ReadLine()));
    return line.Trim();
}

IEnumerable<string> GetRepeatingData(string request)
{
    int count = 0;
    string? line = null;
    do
    {
        if (line != null) yield return line;
        Console.Write($"{request} {++count}: ");
    }
    while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()?.Trim()));
}