using System.Diagnostics;

var watch = Stopwatch.StartNew();
CountToNearlyInfinity(); // <<< Method to benchmark
watch.Stop();
Console.WriteLine(watch.Elapsed);

MeasureTime(CountToNearlyInfinity);
// or
MeasureTime(() => CountToNearlyInfinity());

Console.WriteLine($"The result is {MeasureTimeFunc(() => CalculateSomeResult())}");

/// Functions:

// We want a generic function to benchmark methods
static void MeasureTime(Action a)
{
    var watch = Stopwatch.StartNew();
    a();
    watch.Stop();
    Console.WriteLine(watch.Elapsed);
}

static int MeasureTimeFunc(Func<int> a)
{
    var watch = Stopwatch.StartNew();
    var result = a();
    watch.Stop();
    Console.WriteLine(watch.Elapsed);
    return result;
}

static void CountToNearlyInfinity()
{
    for (int i = 0; i < 500_000_000; i++) ;
}

static int CalculateSomeResult()
{
    // Simulate some calculation
    for (int i = 0; i < 500_000_000; i++) ;
    // return a result
    return 2;
}