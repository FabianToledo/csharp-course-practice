
using ChuckNorrisIo;
using ChuckNorrisIo.Model;

const int MIN_QUANTITY = 1;
const int MAX_QUANTITY = 100;


if (args.Length > 1)
{
    Console.WriteLine("Use only one argument.");
    Usage();
}
int qJokes = 100;
bool clear = false;
if (args.Length == 1)
{
    if (args[0] == "clear")
    {
        clear = true;
    }
    else
    {
        if (!int.TryParse(args[0], out qJokes))
        {
            Console.WriteLine("The quantity has to be a number");
            Usage();
            return;
        }
        if (qJokes > MAX_QUANTITY || qJokes < MIN_QUANTITY)
        {
            Console.WriteLine($"The value has to be between {MIN_QUANTITY} and {MAX_QUANTITY}.");
            Usage();
            return;
        }
    }
}

if (clear)
{
    await Clear();
    return;
}

//await Import(qJokes);
await ImportV2(qJokes);

// //////////////////////////////////////////////////////////////////////

static async Task Clear()
{
    var chuckRepo = new ChuckRepository(new ChuckWitzContextFactory());
    var rows = await chuckRepo.ClearAll();

    Console.WriteLine($"{rows} rows have been deleted");
}

static async Task Import(int qJokes)
{
    var chuckClient = new ChuckApiClient();
    var chuckRepo = new ChuckRepository(new ChuckWitzContextFactory());

    var categories = await chuckClient.GetCategories();
    var filteredCategories = chuckClient.FilterCategories(categories, "explicit", "fashion");

    List<ChuckWitz> jokesToAdd = new();

    for (int i = 0; i < qJokes; i++)
    {
        int retries = 10;
        ChuckWitz aJoke;
        do
        {
            aJoke = await chuckClient.GetRandomJoke(filteredCategories);

        } while (--retries > 0 && (await chuckRepo.JokeExists(aJoke) || jokesToAdd.Any(j => j.ChuckNorrisId == aJoke.ChuckNorrisId)));

        if (retries == 0)
        {
            Console.WriteLine("We already got all the Jokes");
            break;
        }

        jokesToAdd.Add(aJoke);
    }

    await chuckRepo.SaveRange(jokesToAdd);

    Console.WriteLine();
    Console.WriteLine($"{jokesToAdd.Count} Jokes added");

    foreach (var joke in jokesToAdd)
    {
        Console.WriteLine($"{joke.ChuckNorrisId} - {joke.Witz}");
    }
}

static async Task ImportV2(int qJokes)
{
    var chuckClient = new ChuckApiClient();
    var chuckRepo = new ChuckRepository(new ChuckWitzContextFactory());

    var categories = await chuckClient.GetCategories();
    var filteredCategories = chuckClient.FilterCategories(categories, "explicit", "fashion");

    List<ChuckWitz> jokesToAdd = new();
    //IEnumerable<ChuckWitz> allJokes = await chuckRepo.GetAll();
    IAsyncEnumerable<ChuckWitz> allJokes = chuckRepo.GetAllAsyncEnum();

    for (int i = 0; i < qJokes; i++)
    {
        int retries = 10;
        ChuckWitz aJoke;
        do
        {
            aJoke = await chuckClient.GetRandomJoke(filteredCategories);

        //} while (--retries > 0 && (allJokes.Any(j => j.ChuckNorrisId == aJoke.ChuckNorrisId) || jokesToAdd.Any(j => j.ChuckNorrisId == aJoke.ChuckNorrisId)));
        } while (--retries > 0 && (await allJokes.AnyAsync(j => j.ChuckNorrisId == aJoke.ChuckNorrisId) || jokesToAdd.Any(j => j.ChuckNorrisId == aJoke.ChuckNorrisId))) ;

    if (retries == 0)
        {
            Console.WriteLine("We already got all the Jokes");
            break;
        }

        jokesToAdd.Add(aJoke);
    }

    await chuckRepo.SaveRange(jokesToAdd);

    Console.WriteLine();
    Console.WriteLine($"{jokesToAdd.Count} Jokes added");

    foreach (var joke in jokesToAdd)
    {
        Console.WriteLine($"{joke.ChuckNorrisId} - {joke.Witz}");
    }
}

static void Usage()
{
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine();
    Console.WriteLine("ChuckNorrisIo [quantity]");
    Console.WriteLine();
    Console.WriteLine("Quantity: a number between 1 and 10. (default 5). The number of jokes to be imported");
}