
// Data types are separated into value types and reference types.

// Built-in Value types:
using System.Text;

bool aBool = true;
byte aByte = 0;
sbyte aSByte = 0;
char aChar = 'a';
short aShort = 0;
ushort aUShort = 0;
int aInt = 1;
uint aUint = 0;
long aLong = 0;
ulong aULong = 0;
float aFloat = 0;
double aDouble = 0;
decimal aDecimal = 0;
nint aNInt = 0;
nuint aNUint = 0;

// Built-in reference types
string aString = "";
object aObject = 1;
dynamic aDynamic = 1;

// All the Value types inherits from ValueType (Inheritance: Object --> ValueType)
// ValueType overrides the virtual methods from Object with more appropiate implementations for valur types.

// Value types are either stack-allocated or allocated inline in a structure.
// Reference types are heap-allocated.

// Both reference and value types are derived from the ultimate base class Object.
// In cases where it is necesary for a value type to behave like an object, a wrapper that makes the value type
// look like a reference object is allocated on the heap, and the value type's value is copied into it.
// The wrapper is marked so the system knows that it contains a value type.
// This process is known as boxing, and the reverse process is known as unboxing.

// Boxing and unboxing allows any type to be trated as an object.
object aObject2 = aInt; // This boxes the integer into an object and copies its value.
	///* 0x000002B2 1106         */ IL_0056: ldloc.s   aInt
	///* 0x000002B4 8C15000001   */ IL_0058: box       [System.Runtime]System.Int32
	///* 0x000002B9 1312         */ IL_005D: stloc.s   aObject2
	
Console.WriteLine($"A boxed int in an object - value: {aObject2}");

int aInt2 = (int)aObject2; // This unboxes the value into an integer.
	///* 0x000002C3 1112         */ IL_0067: ldloc.s   aObject2
	///* 0x000002C5 A515000001   */ IL_0069: unbox.any [System.Runtime]System.Int32
	///* 0x000002CA 1313         */ IL_006E: stloc.s   aInt2

Console.WriteLine($"The object unboxed in a int - value: {aInt2}");

// Difference between an object and a dynamic object
// There are statically typed languages like C++, Java, Typescript
// and dynamically typed languages like python and javascript
// C# is both, normally is statically typed, but if we declare a variable dynamic
// the type can change inthe middle of execution:

aDynamic = aDynamic + 3;   // The compiler does not checks for the type and let this compile whether causes a run-time error or not
aObject = (int)aObject + 3;// The compiler checks for types and does not compile if it is not casted.
Console.WriteLine();
Console.WriteLine($"object GetType: {aObject.GetType()}");
Console.WriteLine($"object Value: {aObject}");
Console.WriteLine($"dynamic GetType: {aDynamic.GetType()}");
Console.WriteLine($"dynamic Value: {aDynamic}");
aDynamic = "Hola";
Console.WriteLine($"dynamic Value: {aDynamic}");
aDynamic = 4.1;
Console.WriteLine($"dynamic Value: {aDynamic}");

// What are the functions that object supports?
var isEquals = aObject.Equals(aDynamic); // Equals
var repr = aObject.ToString();			 // ToString
var hash = aObject.GetHashCode();		 // GetHashCode
var type = aObject.GetType();			 // GetType
Console.WriteLine();
Console.WriteLine($"Equals:   {isEquals} - Compares the value");
Console.WriteLine($"ToString: {repr}");
Console.WriteLine($"HashCode: {hash}");
Console.WriteLine($"Type:     {type} - Data type");	

var areRefEq = object.ReferenceEquals(aObject, aDynamic);
var areObjEq = object.Equals(aObject, aDynamic);
Console.WriteLine($"areRefEq: {areRefEq} - Compares the references");
Console.WriteLine($"areObjEq: {areObjEq} - Compares the values");


/// struct or class

// Although ValueType is the implicit base class for value types,
// you cannot create a class that inherits from ValueType directly.
// Instead, C# provides the keyword **struct** to support the creation of value types.

// struct is a value type --> it is created in the stack

// class is a reference type --> it is created in the heap

void DoSomething()
{
	var dt = new DateTime(); // DateTime is a struct
	var dt2 = dt; // When we assign a struct it is copied to the new variable.
				  // i.e. we create a new value type object and copy the content from the original variable.

	// In general we use a struct when the amount of data in the fields of a type is relatively small,
	// because struct types are copied when passed to a function, when returned, when assigned, etc.
	// struct are used in multithreading programming (parallel programming)
	// Clean up: C# cleans up the struct when it returns from the method (pops from the function stack)

	var sb = new StringBuilder("Hola"); // StringBuilder is a class
	var sb2 = sb; // When we assign a class, we reference the same object with another variable.
                  // We have 2 references to the same object sb and sb2.
                  // If we change the object it will change in both.

    // class types copies the reference when passed to a funtion, when returned, when assigned, etc.
    // so, if the amount of data in the fields of a type are relatively large we use classes (to use the heap).
    // Clean up: When will C# cleans up the reference type objects?
    // The job is done by the Garbage Collector.
    // The Garbage Collector is intelligent and monitors the heap memory.
    // For instance, if the PC is idle, the GC could decide to run, or if the memory is running critically low, etc.
    // The GC will look at the objects that are not referenced anymore and clean them up.
    // To optimize the performance of the GC, the heap is divided into 3 generations 0,1 and 2.
    // Generation 0: short lived objects (youngest generation).
    // Generation 1 and 2: if the object has survived collections it is promoted to the nexts generations.
    // Large object heap (LOH), sometimes referred to as Generation 3 (it is collected with generation2):
    // stores newly created large objects.

    // We can call the garbage collector explicilty:
    Console.WriteLine("Garbage Collecting...");
	GC.Collect(); // Forces an immediate garbage collection of all generations.
	GC.Collect(1); // Forces an immediate garbage collection from generation 0 through the specified generation.

	// There is a tool to measure the time that the garbage collector runs in your application:
	// perfview https://github.com/microsoft/perfview

}

DoSomething();

