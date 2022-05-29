using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// 5) Create a DbContext using the Factory
var factory = new CookbookContextFactory();
using var context = factory.CreateDbContext(args);

/// 6) Now we can use our context to query, add, update or remove data from the db.

// Add data
Console.WriteLine("Add Kaffee mit Milch for breakfast");

var kaffeeMitMilch = new Dish
{
    Title = "Kaffee mit Milch",
    Notes = "50% Kaffee / 50% Milch",
    Stars = 4,
};

Console.WriteLine($"The ID of this element before adding is {kaffeeMitMilch.Id}");
context.Dishes.Add(kaffeeMitMilch);
var entries = await context.SaveChangesAsync();

Console.WriteLine($"The dish was successfully added to the db (entries saved: {entries})");
Console.WriteLine($"The ID of this element after saving it is {kaffeeMitMilch.Id}");

// Query data
Console.WriteLine();
Console.WriteLine("Checking stars for Kaffee mit Milch");
var dish = await context.Dishes.Where(d => d.Title.Contains("Kaffe")) // LINQ to SQL
                               .FirstOrDefaultAsync();

if (dish == null) { Console.WriteLine("Error!!"); return; }
Console.WriteLine($"The dish {dish.Title} has {dish.Stars} stars");

// Update data
Console.WriteLine();
Console.WriteLine("Changing the Kaffee mit Milch stars field");
kaffeeMitMilch.Stars = 5;
entries = await context.SaveChangesAsync();
Console.WriteLine($"The dish was successfully updated to the db (entries updated: {entries})");

// Remove data
Console.WriteLine();
Console.WriteLine("Removing Kaffee mit Milch from the db");
context.Dishes.Remove(kaffeeMitMilch);
entries = await context.SaveChangesAsync();
Console.WriteLine($"The dish was successfully removed from the db (entries removed: {entries})");



// /////////////////////////////////////////////////////////////////////////////////////////

/// 1) Create the model classes
#region Model
// The model classes will be turn into tables in the DB
// (there are more complex scenarios where the classes does not map one to one with the tables e.g "m-n relationships" or "classes with inheritance")

class Dish
{
    //By convention, all "public" properties with a "getter" and a "setter" will be **included** in the model.

    public int Id { get; set; } // By convention if we call a property of type int "Id",
                                // entity framework will treat it as the primary key and will create the values automatically.
                                // Also it could be named <type>Id

    // How do we tell EF that a value in the DB is
    // * An optional value (the column is Nullable)     --> we use the nullable reference or value type e.g. string? or int?
    // * A mandatory value (the column is Not nullable) --> we use normal reference or value type (without ?) e.g. string or int

    // NOTE: if C# 8 Nullable Reference Types (NRT) is disabled all reference types are optional by default
    // and need the data annotation [Required] to be mandatory. E.g.
    //    public string Title { get; set; }; // OPTIONAL
    //    [Required]
    //    public string Title { get; set; }; // MANDATORY

    // We can use "Data annotations" to add constraints to each column (using System.ComponentModel.DataAnnotations)
    // https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwithout-nrt

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Notes { get; set; }
    public int? Stars { get; set; }

    // Relation between Dish and DishIngredient
    // a Dish has many DishIngredients (one to many)
    // We need to model a foreing key relationship from DishIngredient to the Dish
    // On the n-side of the relation we put a list (many) of DishIngredients
    public List<DishIngredient> DishIngredients { get; set; } = new();
}

class DishIngredient
{
    public int Id { get; set; } // Primary Key with auto-generated value
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(50)]
    public string UnitOfMeasure { get; set; } = string.Empty;
    // To fill the gap between the relational database and .net types we need to add the extra properties that the relational db data types has.
    [Column(TypeName = "decimal(5,2)")] // decimal(5,2) is specific to SQL server DB other may use a diferent data type
    public decimal Amount { get; set; }

    
    // In the 1-side of the relation we need to reference the Dish that this Ingredient relates to.
    public Dish? Dish { get; set; } // This is called a "Navigation property" because it is used to get the related data
    public int DishId { get; set; } // Foreign key for the dish object. Note the naming convention <relation>Id.
}
#endregion

/// 2) Create the database context DbContext
#region DbContext
// the DbContext is the entry point to access the data base,
// it contains a typed version of the database, i.e., contains entry points in order to query, add, update, delete data from every table.
// To use DbContext with SQLServer install the nuget package **Microsoft.EntityFrameworkCore.SqlServer**

class CookbookContext : DbContext
{
    // DbSet represents a table in the database that can be queried / and add, update, delete data
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishIngredient> DishIngredients { get; set; }


    public CookbookContext(DbContextOptions<CookbookContext> options) : base(options)
    { }
}
#endregion

/// 3) Create a Context Factory
#region Factory
// This is only needed if we are starting from scratch, as in a Console Application.
// The Templates for ASP.net (web or api) already have the code for the factory written down.
class CookbookContextFactory : IDesignTimeDbContextFactory<CookbookContext>
{
    public CookbookContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var optionsBuilder = new DbContextOptionsBuilder<CookbookContext>();
        optionsBuilder
            // Logger to print generated SQL statements in console
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"])
            ;
        
        return new CookbookContext(optionsBuilder.Options);
    }
}
#endregion

/// 4) Create the database
#region CreateDatabase
/// 4a) Install dotnet-ef tool
// execute in a console: dotnet tool install --global dotnet-ef

/// 4b) Create the database schema (in the db) from our DbSets, i.e., our declared classes
// This can be generated using migrations
// and we will have to run a new migrations everytime that we change the structure of the DB

// NOTA: correr estos comandos en el proyecto que tiene el modelo, i.e., donde esté la clase que hereda de DbContext
//       para lograr esto, seleccionar en el Package Manager Console: Default project --> Model (suponiendo que el proyecto se llama Model)
/// Install the package Microsoft.EntityFrameworkCore.Design
// dotnet add package Microsoft.EntityFrameworkCore.Design
// or install it in the nuget manager
/// Creamos los archivos que generan la migración y lo nombramos InitialCreation
// dotnet ef migrations add InitialCreation
/// Actualizamos la base de datos local que está conectada al proyecto
// dotnet ef database update

/// 4a and 4b bis - Alternatively we can use the Package Manager Console (PM):
// NOTA: correr estos comandos en el proyecto que tiene el modelo, i.e., donde esté la clase que hereda de DbContext
//       para lograr esto, abrir una consola en el directorio del proyecto Model (suponiendo que el proyecto se llama Model)
/// Install the package Microsoft.EntityFrameworkCore.Tools
// Install-Package Microsoft.EntityFrameworkCore.Tools
/// Creamos los archivos que generan la migración y lo nombramos InitialCreation
// Add-Migration InitialCreation
/// Actualizamos la base de datos local que está conectada al proyecto
// Update-Database

/// EXTRA: Para actualizar una base de datos productiva se puede generar un script SQL que aplica los cambios
/// desde una migración seleccionada hasta otra seleccionada.
/// Por ejemplo, la siguiente línea genera un script con todas las migraciones desde InitialCreationId hasta la última.
// Consola:
// dotnet ef migrations script InitialCreationId
// PM:
// Script-Migration InitialCreationId

/// EXTRA: para remover la última migración:
// Consola:
// dotnet ef migrations remove
// PM:
// Remove-Migration
#endregion
