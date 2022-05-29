
if(args.Length < 2)
{
    Console.WriteLine($"Usage:");
    Console.WriteLine("BarChart GROUPING SUMARIZE [MAX_LINES]");
    Console.WriteLine("");
    Console.WriteLine("GROUPING: Grouping column");
    Console.WriteLine("SUMARIZE: Sumarize column");
    Console.WriteLine("MAX_LINES: Maximum lines (optional)");
    return;
}

// Parse the input arguments
var groupingCol = args[0];
var sumCol = args[1];
var maxLines = args.Length == 3 ? Int32.Parse(args[2]) : 0;

List<Dictionary<string, string>> entries = new();

string? line;
string[] columnsHeaders = new string[4];
line = Console.ReadLine();
if (line != null)
    columnsHeaders = line.Split('\t');

while((line = Console.ReadLine()) != null)
{
    var columns = line.Split('\t');
    entries.Add(new Dictionary<string, string>()
    {
        { columnsHeaders[0], columns[0] },
        { columnsHeaders[1], columns[1] },
        { columnsHeaders[2], columns[2] },
        { columnsHeaders[3], columns[3] },
    });
}

// Una forma: agrupo, luego proyecto a un obj anónimo cada grupo de Dictionary<string,string>,
// hago una suma (agregación) de la clave (key) con nombre {sumCol}
//var grouped = entries.GroupBy(e => e[groupingCol])
//                     .Select(g => new
//                     {
//                         GroupingCol = g.Key,
//                         SumCol = g.Sum(e => int.Parse(e[sumCol]))
//                     });

// Otra forma: agrupo y proyecto el Dictionary a un int con el dato de la clave (key) sumCol,
// luego proyecto a un obj anónimo cada grupo de int
var grouped = entries.GroupBy(e => e[groupingCol], g => int.Parse(g[sumCol]))
                     .Select(g => new
                     {
                         GroupingCol = g.Key,
                         SumCol = g.Sum(e => e)
                     });

// Ordeno
var sorted = grouped.OrderByDescending(a => a.SumCol).ToList();

// If the user added a maxLines argument, limit the result
if (maxLines > 0)
    sorted = sorted.Take(maxLines).ToList();

var maxStrLen = sorted.Max(a => a.GroupingCol.Length);
var maxSumCol = sorted.Max(a => a.SumCol);

var barChart = sorted.Select(a => new
    {
        stringPadded = a.GroupingCol.PadLeft(maxStrLen),
        bar = new String('█', a.SumCol * 100 / maxSumCol)
    });

foreach (var bar in barChart)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"{bar.stringPadded} | ");
    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine($"{bar.bar}");
}

Console.ForegroundColor = ConsoleColor.White;

//while (Console.ReadLine() == null);

