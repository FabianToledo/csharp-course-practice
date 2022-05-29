
/// <summary>
/// ///////////////////////////////////////////////////////////
/// delegates and lambdas
/// </summary>

/// 1st part:

MathOp f; // Declare a variable of type delegate with signature -> int (int, int)

f = Add; // Assign a reference (or pointer) to the funtion Add to the variable f.
Console.WriteLine(f(10, 20));

f = Sub; // Assign a reference (or pointer) to the funtion Sub to the variable f.
//f = new MathOp(Sub); // This code do exactly the same as f = Sub;
Console.WriteLine(f(10, 20));


CalculateAndPrint(3, 3, Add);
CalculateAndPrint(3, 3, Sub);

// Let us replace the Add or Sub references with the complete method we want to execute 
// replacing the name of the method with the keyword delegate and removing the return type
// (the return type is inferred from the body of the funtion).
// delegate(int x, int y) { return x + y; } is an anonymous function
CalculateAndPrint(4, 5, delegate (int x, int y) { return x + y; });

// the last anonymous function can be written as a lambda statement using the fat arrow (=>)
// if we only have a single line of code we can remove the return and the curly braces (curly brackets).
// in this context, the compiler knows the types of the arguments, so we can also remove the types of the arguments.
CalculateAndPrint(5, 4, (x, y) => x + y);

CalculateAndPrint(5, 4, (x, y) => x - y);

CalculateAndPrint(5, 4, (x, y) => x * y);

// Prueba mía para ver el compilado resultante
// Lo compila de la misma forma que los lambdas de arriba
CalculateAndPrint(3, 3, AddLambda);

/// 2nd part: Generics
// Now We can pass a type as a parameter and use it for the types in the arguments
CalculateAndPrintWithGenerics<string>("a", "b", (x, y) => x + y);
CalculateAndPrintWithGenerics<int>(1, 2, (x, y) => x + y);
CalculateAndPrintWithGenerics<bool>(true, true, (x, y) => x && y);

// We don't need to explicitly specify the type, the compiler infers the type from the arguments
CalculateAndPrintWithGenerics("a", "b", (x, y) => x + y);
CalculateAndPrintWithGenerics(1, 2, (x, y) => x + y);
CalculateAndPrintWithGenerics(true, true, (x, y) => x && y);
// CalculateAndPrintWithGenerics("2", 2, (x, y) => x + y); <-- this won't compile because there is not any function that accepts 2 different types


#region FUNCTIONS
/// 1st part
// A function that receives as an argument a delegate of type MathOp
// This is the base of a design pattern called the strategy pattern,
// because we pass in the strategy (how to calculate)
static void CalculateAndPrint(int x, int y, MathOp f)
{
    Console.WriteLine(f(x, y));
}

static int Add(int x, int y)
{
    return x + y;
}

static int Sub(int a, int b)
{
    return a - b;
}

static int AddLambda(int a, int b) => a + b;

/// 2nd part: Generics
static void CalculateAndPrintWithGenerics<T>(T x, T y, Combine<T> f)
{
    Console.WriteLine(f(x, y));
}

#endregion



#region DEFINITIONS
// 1st part:
// Type definition for a cathegory of functions
delegate int MathOp(int x, int y);

/// 2nd part: Generics
// Lambdas with generics
// Generics enables us to pass a "type" to a function as a parameter to that function
delegate T Combine<T>(T a, T b);

#endregion