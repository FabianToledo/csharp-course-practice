
// Synchronous call
var lines = File.ReadAllLines("TextFile1.txt");

//foreach (var line in lines)
//{
//    Console.WriteLine(line);
//}

// Asynchronous call
// It returns a Task
var readTask = File.ReadAllLinesAsync("TextFile1.txt");

Console.WriteLine();
Console.WriteLine(readTask.Status); // WaitingForActivation
Thread.Sleep(1);
Console.WriteLine(readTask.Status); // RanToCompletion (sometimes it will still be waiting for activation.

readTask.Wait(); // This function will block the execution until the Task is finished. This is the same as calling the blocking ReadAllLines()

// How to retrieve the result:
lines = readTask.Result; // This will also block the execution.

//foreach(var line in lines)
//{
//    Console.WriteLine(line);
//}


var rTask = File.ReadAllLinesAsync("TextFile1.txt")
                .ContinueWith( t =>
    {
        // This piece of code will be executed once the Task is completed
        // This type of coding is not used anymore. It was new in C# 4.

        // What happens if there is an exception?
        // If we do not treat it manually, we miss the exception
        // So, we have to check if the IsFaulted property is true
        if (t.IsFaulted)
        {
            foreach(Exception e in t.Exception?.InnerExceptions)
            {
                Console.Error.WriteLine(e.Message);
            }
            return;  
        }

        foreach(var line in t.Result)
        {
            Console.WriteLine(line);
        }
    });
Console.WriteLine();
Console.WriteLine("We continue execution while the task is not done yet.");
Console.WriteLine();

/// With C# 5 it was presented the task-based asynchronous pattern (TAP)
/// and the two keywords async / await.
/// 
// the async / await programming let the code be very similar to the synchronous programming type

async Task ReadFile()
{
    // The next line won't block the execution. The await keyword will yield the execution to the caller
    var lines = await File.ReadAllLinesAsync("TextFile1.txt");
    // and continues its execution from here in the main thread when the Task<string>
    // returned from ReadAllLinesAsync finishes its execution.
    // The async/await creates a state machine that executes in steps (as many steps as await has the function)

    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }
}

await ReadFile();

// Another example:
// Let's simulate a network call with Task.Delay
async Task<int> GetDataFromNetwork()
{
    // Simulates a network call
    await Task.Delay(1000);
    var data = 2; // Simulates the obtained data 
    return data;
}

var netResult = await GetDataFromNetwork();
Console.WriteLine();
Console.WriteLine($"Result data from network: {netResult}");

// Let us use async / await in a lambda expression.
Func<Task<int>> getDataFromNetworkLambda = async () =>
{
    // Simulates a network call
    await Task.Delay(1000);
    var data = 2; // Simulates the obtained data 
    return data;
};

var netResultLambda = await getDataFromNetworkLambda();
Console.WriteLine();
Console.WriteLine($"Result data from network (using lambda): {netResultLambda}");

Console.ReadKey();

