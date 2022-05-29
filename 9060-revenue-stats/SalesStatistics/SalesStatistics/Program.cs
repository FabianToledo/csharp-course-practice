using SalesStatistics;
using SalesStatistics.Entities;

string orderDataFilename;
string mode;
bool displayPerc = false;
if (args.Length >= 2)
{
    orderDataFilename = args[0];
    mode = args[1];
    if(args.Length == 3)
        displayPerc = args[2].Contains("-perc");
}
else
{
#if !DEBUG
    Console.WriteLine("Usage:");
    Console.WriteLine();
    Console.WriteLine("SalesStatistics [order-data] [type] [-perc]");
    Console.WriteLine();
    Console.WriteLine("type");
    Console.WriteLine(" -id      \tDisplay a summarized revenue statistic containing the revenue per order ID");
    Console.WriteLine(" -customer\tDisplay a summarized revenue statistic containing the revenue per customer");
    Console.WriteLine("  -perc   \tDisplays the percentage of the customer's revenue in relation to the total revenue (Only with -customer option)");
    Console.WriteLine();
    return;
#else
    orderDataFilename = "order-data.txt";
    mode = "-customer";
    displayPerc = true;
#endif
}


if (!File.Exists(orderDataFilename))
{
    Console.WriteLine($"{orderDataFilename} does not exist");
    return;
}

using var stream = File.OpenText(orderDataFilename);

string? headerOrder = await stream.ReadLineAsync();
string? headerDetail = await stream.ReadLineAsync();

string? line;
List<Order> orders = new();
Order currentOrder = new();
while((line = await stream.ReadLineAsync()) != null)
{
    if(line.Contains("ORDER"))
    {
        currentOrder = Order.Deserialize(line);
        orders.Add(currentOrder);
    }
    if(line.Contains("DETAIL"))
    {
        currentOrder.Details.Add(Detail.Deserialize(line));
    }
}


if (mode == "-id")
{
    foreach (var entry in Statistics.SummarizeByOrderID(orders))
    {
        Console.WriteLine(entry);
    }
}
else if (mode == "-customer")
{
    foreach (var entry in Statistics.SummarizeByCustomer(orders, displayPerc))
    {
        Console.WriteLine(entry);
    }
}


