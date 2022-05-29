

var heroes = new List<Hero>
{
    new("Wade","Wilson","Deadpool", false),
    new(string.Empty, string.Empty, "Homelander", true),
    new("Bruce","Wayne","Batman", false),
    new(string.Empty, string.Empty, "Stormfront", true),
    new(string.Empty, string.Empty, "Mr. Incredible", false),

};

// 1st part
var result = FilterHeroesWhoCanFly(heroes);
var heroesWhoCanFly = string.Join(", ", result);
Console.WriteLine(heroesWhoCanFly);

// 2nd part
Console.WriteLine();
Console.WriteLine("2nd Part: passing a delegate to the function");
var result2a = FilterHeroes2(heroes, h => h.CanFly);
Console.WriteLine(String.Join(", ", result2a));

var result2b = FilterHeroes2(heroes, h => string.IsNullOrEmpty(h.LastName));
Console.WriteLine(String.Join(", ", result2b));

// 3rd part 
// to IEnumerable
Console.WriteLine();
Console.WriteLine("3rd Part: using IEnumerable and yield return");
var result3a = FilterHeroes3(heroes, h => h.CanFly);
Console.WriteLine(String.Join(", ", result3a));
var result3b = FilterHeroes3(heroes, h => string.IsNullOrEmpty(h.LastName));
Console.WriteLine(String.Join(", ", result3b));

// 4th part 
// using generics
Console.WriteLine();
Console.WriteLine("4th Part: using generics");
var result4a = FilterG(heroes, h => h.CanFly);
Console.WriteLine(String.Join(", ", result4a));
var result4b = FilterG(heroes, h => string.IsNullOrEmpty(h.LastName));
Console.WriteLine(String.Join(", ", result4b));

// We can use the function FilterG for other types of elements and other types of collections:
// The Filter function is very similar to the Where function of LINQ
FilterG(new[] { "Fabi", "Flavi", "Fran", "Matu" }, n => n.StartsWith("M"));
FilterG(new[] { 1, 2, 3, 4, 5, 6, 7 }, n => n % 2 == 0);

// 5th part:
// using Predicate
Filter(new[] { "Fabi", "Flavi", "Fran", "Matu" }, n => n.StartsWith("M"));

//-----------------------------------------------------------

// 1st attempt
List<Hero> FilterHeroesWhoCanFly(List<Hero> heroes)
{
    var resultList = new List<Hero>();
    foreach (var hero in heroes)
    {
        if (hero.CanFly)
        {
            resultList.Add(hero);
        }
    }
    return resultList;
}

// 2nd part
// a better way to reuse the function:
List<Hero> FilterHeroes2(List<Hero> heroes, Filter filter)
{
    var resultList = new List<Hero>();
    foreach (var hero in heroes)
    {
        if (filter(hero))
        {
            resultList.Add(hero);
        }
    }
    return resultList;
}

// 3rd part 
// to IEnumerable
// yield return
IEnumerable<Hero> FilterHeroes3(IEnumerable<Hero> heroes, Filter filter)
{
    foreach (var hero in heroes)
    {
        if (filter(hero))
        {
            yield return hero;
        }
    } 
}

// 4th part 
// to generic
IEnumerable<T> FilterG<T>(IEnumerable<T> list, GenericFilter<T> filter)
{
    foreach (var entry in list)
    {
        if (filter(entry))
        {
            yield return entry;
        }
    }
}

// 5th part 
// using the predefined delegate: Predicate
IEnumerable<T> Filter<T>(IEnumerable<T> list, Predicate<T> filter)
{
    foreach (var entry in list)
    {
        if (filter(entry))
        {
            yield return entry;
        }
    }
}

/// declarations

record Hero(string FirstName, string LastName, string HeroName, bool CanFly);

// 2nd part
// a better way to reuse the function:
delegate bool Filter(Hero hero);

// 4th part
// to generics
delegate bool GenericFilter<T>(T t);
