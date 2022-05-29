
// Closures

// 3. Use the higher order function to get the delegate (factory pattern: we create a factory for the functions)
Func<int, int> calculator = CreateCalculator();

// 4. When the function is executed, why factor can be accessed?
// Answer, because the variable factor is promoted from the stack to the heap.
//         This means that the compiler recognizes that inside the function a variable that is out of the scope of that function.
//         The variable Factor should have a shorter life than the lambda function that uses it.
//         But the lambda function captures the out-of-scope variable <-- This is called ** Closures **
Console.WriteLine(calculator(2));

// 5. Test the "behind the scenes" class and method:
Func<int, int> behind_calculator = CreateCalculator_();
Console.WriteLine(behind_calculator(2));


// Example 2:
(var getData, var setData) = CreateGetSet(23);
Console.WriteLine(getData());
setData(12);
Console.WriteLine(getData());


// Higher order function - Is a function that returns a function or gets a function as a parameter.
Func<int, int> CreateCalculator()
{
    var factor = 2; // 1. This variable originally is created on the stack of the function CreateCalculator

    return n => n * factor; // 2. This lambda function uses the variable factor.
}

// Example 2:
(Func<int>, Action<int>) CreateGetSet(int initialValue)
{
    int val = initialValue;

    return (() => val, (v) => val = v); // val is ** captured ** by the 2 lambda functions. (Put the mouse pointer over the fat arrow to see the captured variables)
}

// What does the compiler do behind the scenes?
// It creates a class containing a field (e.g. factor or val) and a method (e.g. the lambda function)
// e.g. with example 1:

static Func<int, int> CreateCalculator_()
{
    Closure1 newClosureObject = new Closure1() { factor = 2 };
    return newClosureObject.Method1;
}

class Closure1
{
    // Fields
    public int factor;

    // Methods
    public int Method1(int n) => n * factor;

} // end of class Closure1




