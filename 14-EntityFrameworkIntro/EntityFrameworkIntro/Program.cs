using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

/// 5) Create a DbContext using the Factory
var factory = new CookbookContextFactory();

//await ModifyEntity(factory);
//await EntityStates(factory);
//await ChangeTraking(factory);
//await AttachedEntities(factory);
//await NoTracking(factory);
//await RawSql(factory);
//await Transactions(factory);
await ExpressionTree(factory);

// How EF translates C# code to SQL??
static async Task ExpressionTree(CookbookContextFactory factory)
{
    using var context = factory.CreateDbContext();
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };

    // Insert the data from newDish
    context.Dishes.Add(newDish);
    await context.SaveChangesAsync();

    var dishes = await context.Dishes
        .Where(d => d.Title.StartsWith("F"))
        .ToArrayAsync();

    // How EF translates the expression we have just written in a SQL query?
    //SELECT[d].[Id], [d].[Notes], [d].[Stars], [d].[Title]
    //  FROM[Dishes] AS[d]
    //  WHERE[d].[Title] LIKE N'F%'

    // Normally the C# compiler, compiles the code to Intermediate Language (IL) and the Just in time compiler (JIT)
    // compiles th IL to machine (assembly) language and the is executed somewhere in memory. This is not what happens!!

    // d => d.Title.StartsWith("F"); // This is a lambda expression

    Func<Dish, bool> f = d => d.Title.StartsWith("F");
    // Entity Framework has no idea how to interpret the method f.

    // .net has the following concept to translate the lambda expression to SQL 
    // The extension methods for IQueryable accepts the type Expression<Tlambda>
    Expression<Func<Dish, bool>> ex = d => d.Title.StartsWith("F");
    // The Expression is compiled to an object tree describing the C# code 
    // that has been written.
    // EF can inspect this object tree at runtime and translate it into SQL (or diferent language)
     
}

static async Task ModifyEntity(CookbookContextFactory factory)
{
    using var context = factory.CreateDbContext();

    /// 6) An experiment...
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };

    // Insert the data from newDish
    context.Dishes.Add(newDish);
    await context.SaveChangesAsync();

    // Generated SQL statement
    // INSERT INTO[Dishes] ([Notes], [Stars], [Title])
    //       VALUES(@p0, @p1, @p2);
    // SELECT[Id]
    //       FROM[Dishes]
    //       WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

    // Change something in the object
    newDish.Notes = "Baz";
    await context.SaveChangesAsync();

    // Generated SQL statement
    // UPDATE[Dishes] SET[Notes] = @p0
    //       WHERE[Id] = @p1;
    // SELECT @@ROWCOUNT;   
}
// How does EF knows that we have updated the object??
// Wherever I have changed, on any object that is in memory, magically write it to the database??

// We will explore how EF does it.
static async Task EntityStates(CookbookContextFactory factory)
{
    using var dbContext = factory.CreateDbContext();
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };

    // Behind the scenes, EF has a change tracker, a mechanism that tracks changes to objects that already knows
    var state = dbContext.Entry(newDish).State; // << Detached --> The entity is not being tracked by the context.

    dbContext.Dishes.Add(newDish);
    state = dbContext.Entry(newDish).State; // << Added --> The entity is being tracked by the context but does not yet exist in the database.

    await dbContext.SaveChangesAsync();

    state = dbContext.Entry(newDish).State; // << Unchanged --> // The entity is being tracked by the context and exists in the database. 
                                                                // Its property values have not changed from the values in the database.

    newDish.Notes = "Baz";
    state = dbContext.Entry(newDish).State; // << Modified --> // The entity is being tracked by the context and exists in the database. 
                                                               // Some or all of its property values have been modified.

    await dbContext.SaveChangesAsync();

    dbContext.Dishes.Remove(newDish);
    state = dbContext.Entry(newDish).State; // << Deleted --> // The entity is being tracked by the context and exists in the database. 
                                                              // It has been marked for deletion from the database.
    await dbContext.SaveChangesAsync();
    state = dbContext.Entry(newDish).State; // << Detached --> The entity is not being tracked by the context.

    // Summary
    // EntityState.Detached  // The entity is not being tracked by the context.
    // EntityState.Unchanged // The entity is being tracked by the context and exists in the database. 
    //                       // Its property values have not changed from the values in the database.
    // EntityState.Deleted   // The entity is being tracked by the context and exists in the database. 
    //                       // It has been marked for deletion from the database.
    // EntityState.Modified  // The entity is being tracked by the context and exists in the database. 
    //                       // Some or all of its property values have been modified.
    // EntityState.Added     // The entity is being tracked by the context but does not yet exist in the database.

}

// Explore in deeper detail the ChangeTracking
static async Task ChangeTraking(CookbookContextFactory factory)
{
    using var dbContext = factory.CreateDbContext();
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };

    dbContext.Dishes.Add(newDish);
    await dbContext.SaveChangesAsync();

    newDish.Notes = "Baz";

    var entry = dbContext.Entry(newDish);

    var originalValue = entry.OriginalValues[nameof(Dish.Notes)]?.ToString();

    // How does EF detects changes:
    // It saves the original value that EF knows that came from the database or 
    // has been written previously on the database
    // When we change something on the tracked object and the call SaveChanges
    // EF compares the original value and the value of the object,
    // If there is a difference, it means that the data has been updated (modified)
    // Checks for each property, what the original value is and what the new value is.
    // With that information can build the correct SQL statement that can run in the database.
    Console.WriteLine();
    Console.WriteLine($"Original value: {originalValue}");
    Console.WriteLine($"New value:      {newDish.Notes}");

    // The change tracker always belongs to a single dbContext.
    using var dbContext2 = factory.CreateDbContext();
    // We will read from the database the recently added newDish using both dbContexts
    var dishFromDatabase = await dbContext.Dishes.SingleAsync(d => d.Id == newDish.Id);
    var dishFromDatabase2 = await dbContext2.Dishes.SingleAsync(d => d.Id == newDish.Id);

    Console.WriteLine();
    Console.WriteLine($"Retrieve newDish using dbContext:  {dishFromDatabase.Notes}");  // Baz
    Console.WriteLine($"Retrieve newDish using dbContext2: {dishFromDatabase2.Notes}"); // Bar

    // This result has various implications:
    // * means that when we want to re-read (with a query) the database of an entry already in the 
    //   change tracking of the dbContext, EF will return the in-memory value.
    // * means that the change tracking of the second dbContext does not know of the existence of that object
    //   and reads it from the database.
    // * The change tracking is not global and is isolated to a specific dbContext.

}

// Detaching an entity. Using Update to Add (if not PK present) or Modify (if PK present).
static async Task AttachedEntities(CookbookContextFactory factory)
{
    var dbContext = factory.CreateDbContext();

    var newDish = new Dish { Title = "Foo", Notes = "Bar" };

    // We can use Update to Add or Update an entity
    // If the entity has a primary key, the Update method will do different things if the value is set or not.
    // 1) if Id (primary key) is not set (CLR default for the property type), the tracker will set the state to EntityState.Added
    dbContext.Dishes.Update(newDish); // >> EntityState.Added (Note the generated INSERT SQL code)
    await dbContext.SaveChangesAsync(); // >> EntityState.Unchanged
    var state = dbContext.Entry(newDish).State;
    //INSERT INTO[Dishes] ([Notes], [Stars], [Title])
    //  VALUES(@p0, @p1, @p2);
    //SELECT[Id]
    //  FROM[Dishes]
    //  WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

    // How can we tell to EF forget an entity
    // Setting the state property to Detached will tell EF: do not track this object anymore.
    dbContext.Entry(newDish).State = EntityState.Detached; // >> EntityState.Detached
    state = dbContext.Entry(newDish).State;

    // 2) if Id (primary key) is set, the tracker will set the state to EntityState.Modified
    // The UPDATE Sql command generated will update all the fields.
    dbContext.Dishes.Update(newDish); // EntityState.Modified (Note the generated UPDATE SQL code)
    await dbContext.SaveChangesAsync();
    //UPDATE[Dishes] SET[Notes] = @p0, [Stars] = @p1, [Title] = @p2
    //  WHERE[Id] = @p3;
    //SELECT @@ROWCOUNT;

}

// No tracking queries
static async Task NoTracking(CookbookContextFactory factory)
{
    var dbContext = factory.CreateDbContext();
    
    // Analog to SELECT * FROM dishes
    var dishes = await dbContext.Dishes.ToArrayAsync();
    // EF adds to the ChangeTracker all the elements of the query
    var state = dbContext.Entry(dishes[0]).State;  // >> EntityState.Unchanged

    // If we do not need to track the changes because we will not change the data
    // e.g. if we will only print the data, or for a report, or show the data in an html page
    // We need to add AsNoTracking() before the ToArrayAsync()
    var dishesNotTracked = await dbContext.Dishes.AsNoTracking().ToArrayAsync();
    state = dbContext.Entry(dishesNotTracked[0]).State; // >> EntityState.Detached

}

// Generating a Raw SQL statment
static async Task RawSql(CookbookContextFactory factory)
{
    using var dbContext = factory.CreateDbContext();

    // We can inject RAW SQL statements and the returned elements will be treated
    // equaly as a CLR object and we can compose on top using LINQ operators, i.e, it returns an IQueryable,
    // and the change tracker will work on those object.
    var dishes = await dbContext.Dishes
        .FromSqlRaw("SELECT * FROM dishes")
        .ToArrayAsync();

    // How to pass parameters to the RAW SQL?
    var filter = "%z";

    // 1st option include parameters place holders, these will automatically be converted to DbParameter
    dishes = await dbContext.Dishes
        .FromSqlRaw("SELECT * FROM dishes WHERE Notes LIKE {0}", filter)
        .ToArrayAsync();
    // 2nd option include named place holders and DbParameter instance as parameter value
    dishes = await dbContext.Dishes
        .FromSqlRaw("SELECT * FROM dishes WHERE Notes LIKE @p0", new Microsoft.Data.SqlClient.SqlParameter("@p0", filter))
        .ToArrayAsync();

    // NEVER USE INTERPOLATION IN A FromSqlRaw. THIS MAY LEAD TO AN INJECTION ATTACK.
    dishes = await dbContext.Dishes
        .FromSqlRaw($"SELECT * FROM dishes WHERE Notes LIKE '{filter}'") // BAD !!!! <-- code inyection
        .ToArrayAsync();

    // To use interpolation, we can use FromSqlInterpolated.
    // This method will automatically convert any interpolated values to DbParameter
    dishes = await dbContext.Dishes
        .FromSqlInterpolated($"SELECT * FROM dishes WHERE Notes LIKE {filter}")
        .ToArrayAsync();

    ////////
    // Writing data to the database using Raw statements
    // We can also pass parameters to our statement using ExecuteSqlInterpolatedAsync
    var id = 70;
    await dbContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Dishes WHERE Id = {id}");

    // With this statement we will delete all the dishes that do not have ingredients
    await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Dishes WHERE Id NOT IN (SELECT DishId FROM DishIngredients)");

}

// Transactions
// We want to create an entire sequence of operations 
static async Task Transactions(CookbookContextFactory factory)
{
    using var dbContext = factory.CreateDbContext();

    // We start a transaction using the BeginTransaction method of dbContext.Database
    using var transaction = await dbContext.Database.BeginTransactionAsync();
    // We must put the block inside a try catch and execute Commit at the end of the try
    try
    {
        dbContext.Dishes.Add(new Dish { Title = "Foo", Notes = "Bar" });
        await dbContext.SaveChangesAsync();

        // If we execute this statement will throw an exception, so we do not reach to the commit.
        await dbContext.Database.ExecuteSqlRawAsync("SELECT 1/0 as Bad");

        // If all the above statements executed correctly, we commit the transaction.
        await transaction.CommitAsync();
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Something Bad happened: {ex.Message}");
        // If there is an exception
        // We Rollback the changes with Rollback
        await transaction.RollbackAsync();
    }
}



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
    public CookbookContext CreateDbContext(string[]? args = null)
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
