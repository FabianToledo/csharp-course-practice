using CsvHelper;
using System.Globalization;
using FindingDelta.logic;
using CsvHelper.Configuration;

string newDataFilename;
string oldDataFilename;

if (args.Length == 2)
{
    oldDataFilename = args[0];
    newDataFilename = args[1];
}
else
{
#if DEBUG
    newDataFilename = "data-new.txt";
    oldDataFilename = "data-old.txt";
#else
    Console.WriteLine("Usage:");
    Console.WriteLine("FindDelta [old_data_filename] [new_data_filename]");
    Console.WriteLine();
    return;
#endif
}

if (!File.Exists(newDataFilename))
{
    Console.WriteLine($"{newDataFilename} does not exist. Please check the name of the file");
    return;
}
if (!File.Exists(oldDataFilename))
{
    Console.WriteLine($"{oldDataFilename} does not exist. Please check the name of the file");
    return;
}

var newRecords = await GetRecords(newDataFilename);
var oldRecords = await GetRecords(oldDataFilename);

foreach (var delta in DeltaFinder.Deltas(oldRecords, newRecords))
{
    Console.WriteLine(delta);
}

static async Task<List<Data>> GetRecords(string filename)
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t"
    };

    using var reader = new StreamReader(filename);
    using var csv = new CsvReader(reader, config);
    return await csv.GetRecordsAsync<Data>().ToListAsync();
}

