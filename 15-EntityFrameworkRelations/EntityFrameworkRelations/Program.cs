using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

var factory = new BrickContextFactory();

//await AddData(factory);
//Console.WriteLine("Done!");

//await QueryData(factory);

await QueryDataLater(factory);


static async Task AddData(BrickContextFactory factory)
{
    using var context = factory.CreateDbContext();

    Vendor brickKing, heldDerSteine;
    var vendors = new Vendor[]
    {
        brickKing = new() { VendorName = "Brick King" },
        heldDerSteine = new() { VendorName = "Held der Steine" },
    };
        
    await context.Vendors.AddRangeAsync(vendors);
    await context.SaveChangesAsync();

    Tag rare, ninjago, minecraft;
    var tags = new Tag[]
    {
        rare = new() { Title = "Rare" },
        ninjago = new() { Title = "Ninjago" },
        minecraft = new() { Title = "Minecraft" },
    };

    await context.Tags.AddRangeAsync(tags);
    await context.SaveChangesAsync();

    await context.AddAsync(new BasePlate
    {
        Title = "BasePlate 16 x 16 with blue water pattern",
        BrickColor = Color.Grun,
        Tags = new() { rare, minecraft },
        Length = 16,
        Width = 16,
        BrickAvailabilities = new()
        {
            new() { Vendor = brickKing, AvailableAmount = 5, PriceEur = 6.5m },
            new() { Vendor = heldDerSteine, AvailableAmount = 10, PriceEur = 5.9m },
        }
    });
    await context.SaveChangesAsync();
}

static async Task QueryData(BrickContextFactory factory)
{
    using var context = factory.CreateDbContext();

    var availabilityData = await context.BrickAvailabilities
        .Include(ba => ba.Brick)  // Include Enforces to generate a Join to get the related data
        .Include(ba => ba.Vendor) // Include Enforces to generate a Join to get the related data
        .ToArrayAsync();

    availabilityData = await context.BrickAvailabilities
        //!note that this is a 2 step join and will get the brick too!
        .Include(ba => ba.Brick.Tags) // Include Enforces to generate a Join to get the related data, 
        .Include(ba => ba.Vendor) // Include Enforces to generate a Join to get the related data
        .ToArrayAsync();

    foreach (var item in availabilityData)
    {
        Console.WriteLine($"Brick {item.Brick.Title} available at {item.Vendor.VendorName} for {item.PriceEur}");
    }
    Console.WriteLine();

    var bricks = await context.Bricks
        .Include(b => b.Tags)
        .ToArrayAsync();

    foreach (var brick in bricks)
    {
        Console.WriteLine($"Brick {brick.Title}");
        if (brick.Tags.Any())
        {
            Console.WriteLine($" Tags: {string.Join(',', brick.Tags.Select(t => t.Title))}");
        }
    }

    var bricksImplicitInclude = await context.Bricks
        .Where(b => b.Tags.Any(t => t.Title == "Minecraft")) // Implicitly includes the tags data because we are querying that data.
        .ToArrayAsync();

    var bricksWithVendorAndTags = await context.Bricks
        // Para realizar un doble salto, se puede lograr utilizando la sobrecarga que 
        // acepta string navigationPropertyPath, que es un string con el path hasta la propiedad deseada (separada con puntos '.')
        .Include(nameof(Brick.BrickAvailabilities) + "." + nameof(BrickAvailability.Vendor))
        .Include(b => b.Tags)
        .ToArrayAsync();

    foreach (var brick in bricks)
    {
        Console.WriteLine($"Brick {brick.Title}");
        if (brick.Tags.Any())
            Console.WriteLine($" Tags: {string.Join(',', brick.Tags.Select(t => t.Title))}");
        if (brick.BrickAvailabilities.Any())
            Console.WriteLine($" Is available at: {string.Join(',', brick.BrickAvailabilities.Select(ba => ba.Vendor.VendorName))}");
    }
}

static async Task QueryDataLater(BrickContextFactory factory)
{
    using var context = factory.CreateDbContext();

    Console.WriteLine();

    var simpleBricks = await context.Bricks.ToArrayAsync();
    // At this point the tags data is not loaded from the DB
    foreach (var brick in simpleBricks)
    {
        // We can ask to include the information for specific elements
        await context.Entry(brick).Collection(b => b.Tags).LoadAsync();

        Console.WriteLine($"Brick {brick.Title}");
        if (brick.Tags.Any())
            Console.WriteLine($" Tags: {string.Join(',', brick.Tags.Select(t => t.Title))}");
    }

}

// 1) Build the data model
#region Model
public enum Color
{
    Schwarz,
    Weis,
    Rot,
    Gelb,
    Orange,
    Grun,
}
public class Brick
{
    public int Id { get; set; }
    [MaxLength(250)]
    public string Title { get; set; } = string.Empty;
    public Color? BrickColor { get; set; }

    public List<Tag> Tags { get; set; } = new();
    public List<BrickAvailability> BrickAvailabilities { get; set; } = new();
}
// many-to-many relations for Brick-Tag. From EF 5.0 we do not have to create an intermediate class.
// EF will create it for us.
public class Tag
{
    public int Id { get; set; }
    [MaxLength(250)]
    public string Title { get; set; } = string.Empty;
    public List<Brick> Bricks { get; set; } = new();

}

// Inheritance: 
// By default EF will put the base class and the derived classes into a single table.
// Table per hierarchy: the entire hierarchy is store in a signle table.
public class BasePlate : Brick
{
    public int Length { get; set; }
    public int Width { get; set; }
}

public class MinifigHead : Brick
{
    public bool IsDualSided { get; set; }

}

public class Vendor
{
    public int Id { get; set; }
    [MaxLength(250)]
    public string VendorName { get; set; } = string.Empty;
    public List<BrickAvailability> BrickAvailabilities { get; set; } = new();
}  

public class BrickAvailability
{
    public int Id { get; set; }
    public Vendor Vendor { get; set; }
    public int VendorId { get; set; }
    public Brick Brick { get; set; }
    public int BrickId { get; set; }
    public int AvailableAmount { get; set; }
    [Column(TypeName = "decimal(8,2)")]
    public decimal PriceEur { get; set; }
}
#endregion

// 2) Create the data context
#region DbContext
public class BrickContext : DbContext
{
    public BrickContext(DbContextOptions<BrickContext> options) : base(options) { }
    
    public DbSet<Brick> Bricks { get; set; }
    // Si se agregan las clases que heredan, se va a crear una tabla conteniendo
    // las tres clases y una columna discriminator que almacena el tipo al que pertenece
    // la fila.
    public DbSet<BasePlate> BasePlates { get; set; }
    public DbSet<MinifigHead> MinifigHeads { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<BrickAvailability> BrickAvailabilities { get; set; }


    // Model configuration using fluent API
    // We are going to configure the behavior of inheritance in the model
    // in this case we are going to use 1 table to store the 3 classes that have inheritance:
    // Brick --> BasePlate and MinifigHead
    // This is the default and is called Table-per-hierarchy (TPH)
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    // EDIT: from microsoft page:
    //    // If you don't rely on conventions, you can specify the base type explicitly using HasBaseType.
    //    // You can also use .HasBaseType((Type)null) to remove an entity type from the hierarchy.
    //    // NOTA: según la pag de microsoft para que aparezcan las entidades de las clases derivadas,
    //    // solamente hay que exponerlas utilizando DbSet y no haría falta agregarlas aquí.
    //    // Esto sería solamente si no confiamos en las convenciones.
    //    // Lo revisé y se crea exactamente la misma base de datos. Así que comento esto y agrego los DbSet
    //    modelBuilder.Entity<BasePlate>().HasBaseType<Brick>();
    //    modelBuilder.Entity<MinifigHead>().HasBaseType<Brick>();
    //}
}

#endregion

#region DbContextFactory
public class BrickContextFactory : IDesignTimeDbContextFactory<BrickContext>
{
    public BrickContext CreateDbContext(string[]? args = null)
    {
        /// A) Forma muy simple para entender como hacer:
        #region FormaSimple
        //var options = new DbContextOptionsBuilder<BrickContext>()
        //    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Legobricks;Trusted_Connection=True") // Connection string
        //    .Options; // Obtenemos las opciones del builder

        //return new BrickContext(options);
        #endregion

        /// B) Ahora utilizamos el archivo appsettings.json para tomar la configuración del connection string
        /// y también agregamos el logger a consola utilizando Microsoft.Extensions.LoggerFactory
        #region ConAppSettings
        // Creamos un objeto configuration utilizando Microsoft.Extensions.ConfigurationBuilder
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<BrickContext>()
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;
        #endregion

        return new BrickContext(options);
    }
}

#endregion