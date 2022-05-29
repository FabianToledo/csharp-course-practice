
using System.Text.Json;
using System.Text.Json.Serialization;

// To generate the data, go to: https://www.mockaroo.com/

// Read all the data from the file "data.json"
// Note, to use a relative path: in properties of the file data.json choose -> Copy to output directory: copy if newer
var fileContent = await File.ReadAllTextAsync(@"data.json");

// Deserialize the content to an object
var cars = JsonSerializer.Deserialize<List<CarData>>(fileContent);

// If the list is null create an empty list
cars ??= new List<CarData>();

// Print all cars with at least 4 doors
var carsWithAtLeast4Doors = cars.Where(c => c.NumberOfDoors >= 4);

//foreach (var car in carsWithAtLeast4Doors)
//{
//    Console.WriteLine($"{car.Make} - {car.Model} - {car.NumberOfDoors}".PadLeft(30));
//}

// Print all Mazda cars with at least 4 doors
var mazdasWithAtLeast4Doors = cars.Where(c => c.NumberOfDoors >= 4)
                                  .Where(c => c.Make.Contains("Mazda"));

//foreach (var car in mazdasWithAtLeast4Doors)
//{
//    Console.WriteLine($"{car.Make} - {car.Model} - {car.NumberOfDoors}".PadLeft(30));
//}

// Print  Make + Model for all Makes that starts with M using a Select (projection), and ForEach of a list
cars.Where(c => c.Make.StartsWith("M"))
    .Select(c => $"{c.Make} - {c.Model}")
    .ToList()
    //.ForEach(c => Console.WriteLine(c))
    ;

// Example using my extension method of ForEach
cars.Where(c => c.Make.StartsWith("M"))
    .Select(c => $"{c.Make} - {c.Model}")
    //.ForEach(c => Console.WriteLine(c))
    ;

// Display a List of the 10 most powerful cars (in terms of HP)
cars.OrderByDescending(c => c.HP)
    .Take(10)
    .Select(c => $"{c.HP} - {c.Make} - {c.Model}")
    //.ForEach(c => Console.WriteLine(c))
    ;

// /////////////////////////////////////////////////////////////////
// GroupBy

/// Display the number of models per make that appeared after 1995
cars.Where(c => c.Year > 1995)
    .GroupBy(c => c.Make)
    .Select(c => new
    {
        Make = c.Key,
        Count = c.Count(),
    })
    //.ForEach(c => Console.WriteLine($"{c.Make} - {c.Count}"))
    ;

// or we could project it the same way as the others
cars.Where(c => c.Year > 1995)
    .GroupBy(c => c.Make)
    .Select(c => $"{c.Key} - {c.Count()}")
    //.ForEach(c => Console.WriteLine(c))
    ;

/// Display the number of models per make that appeared after 1995 and
// Display cero if there is no models after that year
// The trick is to move the Where clause inside the select clause where we have
// each group available (Note: IGrouping is an IEnumerable with a Key).
cars.GroupBy(c => c.Make)
    .Select(g => $"{g.Key} - {g.Where(c => c.Year > 1995).Count()}")
    //.ForEach(c => Console.WriteLine(c))
    ;

// The Count method has an overload that takes a function to filter,
// so we can simplify the g.Where.Count
cars.GroupBy(c => c.Make)
    .Select(g => $"{g.Key} - {g.Count(c => c.Year > 1995)}")
    //.ForEach(c => Console.WriteLine(c))
    ;

/// Display a list of Makes that have at least 10 models that appeared after 2000
cars.GroupBy(c => c.Make)
    .Where(g => g.Where(c => c.Year > 2000).DistinctBy(c => c.Model).Count() > 10)
    //.ForEach(g => Console.WriteLine($"{g.Key}"))
    ;

/// Display a list of Makes that have at least 2 models with >= 400 HP
cars.GroupBy(c => c.Make)
    .Where(g => g.Where(c => c.HP >= 400).DistinctBy(c => c.Model).Count() > 5)
    //.ForEach(g => Console.WriteLine($"{g.Key}"))
    ;

// Another way to do it
cars.Where(c => c.HP >= 400)
    .GroupBy(c => c.Make)
    .Select(g => new { Make = g.Key, NumberOfPowerfulCars = g.DistinctBy(c => c.Model).Count() })
    .Where(a => a.NumberOfPowerfulCars > 5)
    //.ForEach(a => Console.WriteLine($"{a.Make}"))
    ;

// Display the average HP per make
cars.GroupBy(c => c.Make)
    //.ForEach(g => Console.WriteLine($"{g.Key} - {g.Average(c => c.HP)}"))
    ;

// Another way to do it
cars.GroupBy(c => c.Make)
    .Select(g => new { Make = g.Key, AverageHP = g.Average(c => c.HP) })
    //.ForEach(a => Console.WriteLine($"{a.Make} - {a.AverageHP}"))
    ;

// How many makes build cars with HP 0..100, 101..200, 201..300, 301..400, 401..500
cars.GroupBy(c => c.HP switch
{
    <= 100 => "0..100",
    <= 200 => "101..200",
    <= 300 => "201..300",
    <= 400 => "301..400",
    _ => "401..500"
}).OrderBy(g => g.Key)
  //.ForEach(g => Console.WriteLine($"{g.Key} - {g.Select(c => c.Make).Distinct().Count()}"))
  .ForEach(g => Console.WriteLine($"{g.Key} - {g.DistinctBy(c => c.Make).Count()}"))
  ;

Console.WriteLine();

// Otra forma
cars.GroupBy(c => c.HP switch
{
    <= 100 => "0..100",
    <= 200 => "101..200",
    <= 300 => "201..300",
    <= 400 => "301..400",
    _ => "401..500"
}).OrderBy(g => g.Key)
  .Select(g => new {g.Key, Makes = g.GroupBy(c => c.Make).Count()})
  .ForEach(a => Console.WriteLine($"{a.Key} - {a.Makes}"));

class CarData
{
    // We need to let .net the name of the property in the json
    // We can do it using the JsonPropertyNameAttribute from System.Text.Json.Serialization
    [JsonPropertyName("id")]
    public int ID { get; set; }
    [JsonPropertyName("car_make")]
    public string Make { get; set; } = string.Empty;
    [JsonPropertyName("car_model")]
    public string Model { get; set; } = string.Empty;
    [JsonPropertyName("car_year")]
    public int Year { get; set; }
    [JsonPropertyName("number_of_doors")]
    public int NumberOfDoors { get; set; }
    [JsonPropertyName("hp")]
    public int HP { get; set; }

}

// This was written by me. Is a simple extension method to add ForEach functionality to IEnumerable
static class EnumerableExt
{
    public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
    {
        foreach (var item in e)
        {
            action(item);
        }
    }
}