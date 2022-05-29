using Protokollanalyse.Logik;
using System.Text.Json;

Mode mode = Mode.None;
if (args.Length >= 1)
{
    mode = args[0] switch
    {
        "--monthly" => Mode.Monthly,
        "--hourly" => Mode.Hourly,
        "--photographers" => Mode.Photographers,
        _ => Mode.None
    };
}

string? imageFilter = null;
if (args.Length == 2)
{
    imageFilter = args[1];
}

// Reads log entries from access-log file
string accessLogFilename = "access-log.txt";
using var reader = File.OpenText(accessLogFilename);
string? headers = await reader.ReadLineAsync();
string? content;
List<LogEntry> logEntries = new();
while ((content = await reader.ReadLineAsync()) != null)
{
    logEntries.Add(LogEntry.FromFileLine(content));
}

switch (mode)
{
    case Mode.Monthly:
        foreach (var line in Analyzer.SummarizeMonthly(logEntries, imageFilter))
        {
            Console.WriteLine(line);
        }
        break;
    case Mode.Hourly:
        foreach (var line in Analyzer.SummarizeHourly(logEntries, imageFilter))
        {
            Console.WriteLine(line);
        }
        break;
    case Mode.Photographers:
        // Reads photographers from Json file
        string photographersFilename = "photographers.json";
        string photographers = await File.ReadAllTextAsync(photographersFilename);
        List<Photo> photos = JsonSerializer.Deserialize<List<Photo>>(photographers) ?? new List<Photo>();

        foreach (var line in Analyzer.SummarizePhotographers(logEntries, photos))
        {
            Console.WriteLine(line);
        }
        break;
    default:
        Console.WriteLine("Usage:");
        Console.WriteLine("Protokollanalyse --[option] [photo_filter]");
        Console.WriteLine("");
        Console.WriteLine("option:");
        Console.WriteLine("\tmonthly\t\tSummarizes by Month");
        Console.WriteLine("\thourly\t\tSummarizes by Hour");
        Console.WriteLine("\tphotographers\tSummarizes by Photographers");
        Console.WriteLine();
        Console.WriteLine("photo_filter: Name of the photo to filter by");

        break;
}












