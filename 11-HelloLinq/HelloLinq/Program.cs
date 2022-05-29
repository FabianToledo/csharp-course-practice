using System.Collections;

IEnumerable enumerable = new int[] { 1, 2, 3, 4, 5 };

// The interface IEnumerable only contains one member GetEnumerator
var enumerator = enumerable.GetEnumerator();

// The interface IEnumerator contains 1 member: the current object
// and 2 methods MoveNext and Reset

while (enumerator.MoveNext())
{
    Console.WriteLine(enumerator.Current);
}

Console.WriteLine();

enumerator.Reset();
enumerator.MoveNext();
Console.WriteLine(enumerator.Current);
enumerator.MoveNext();
Console.WriteLine(enumerator.Current);

// So, IEnumerable is a read only, forward only collection
// We cannot manipulate an IEnumerable, go back, jump, access with an index

Console.WriteLine();
/// Let us write a simple IEnumerable generator 
//
var even = GenerateNumbersBad(10) // The function is called at this point of the program
    // But the where is called we the program gets to the in statement of the foreach loop
    .Where(n => 
    {
        return n % 2 == 0; 
    });

foreach (var n in even)
{
    Console.WriteLine(n);
}

Console.WriteLine();
var evenG = GenerateNumbersGood(10) // The function is not called at this point of the program
    .Where(n =>
    {
        return n % 2 == 0;
    })
    .Select(n =>
    {
        return n * 3;
    });

foreach (var n in evenG) // the function GenerateNumbersGood is called when we get to the 'in' statement
{
    Console.WriteLine(n);
}

Console.WriteLine();
Console.WriteLine($"Count: {evenG.Count()}");

Console.WriteLine();
var res = GenerateNumbersGood(10)
    .Where(n =>
    {
        return n % 2 == 0;
    })
    // When we add an OrderBy clause, the execution is still deferred
    // but it executes with the first MoveNext all the Where statement, all the OrderBy and one time the Select statement
    // the next MoveNext executes only the Select
    .OrderByDescending(n =>
    { 
        return n;
    })
    .Select(n =>
    {
        return n * 3;
    });

foreach (var n in res) // the function GenerateNumbersGood is called when we get to the 'in' statement
{
    Console.WriteLine(n);
}

/// <summary>
/// not good code, all the numbers are generated and stored in memory all at once
/// </summary>
IEnumerable<int> GenerateNumbersBad(int maxValue)
{
    var result = new List<int>();
    for (var i = 0; i < maxValue; i++)
    {
        result.Add(i);
    }
    return result;
}

/// <summary>
/// A beter code, each number is generated only when it is needed.
/// </summary>
IEnumerable<int> GenerateNumbersGood(int maxValue)
{
    for (var i = 0; i < maxValue; i++)
    {
        yield return i;
    }
}