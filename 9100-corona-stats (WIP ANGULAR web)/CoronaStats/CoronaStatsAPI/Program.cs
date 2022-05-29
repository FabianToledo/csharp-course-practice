using CoronaStatsAPI.Services;
using CoronaStatsModel;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    /// Instead of using the Attribute [JsonIgnore] in the data model we can just ignore all the circular references
    /// This only works if we return the object directly in the controller, this does not configure JsonSerializer.Serialize method
    .AddJsonOptions(opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connextionString = builder.Configuration.GetConnectionString("PrimaryDb");
// Shortcut for configuring DbContext with SQL server
builder.Services.AddSqlServer<CoronaStatsDataContext>(
    connextionString,
    
    //// Para separar el DbContext en una librería (en este caso CoronaStatsModel) hay que referenciar al ensamblado que lo contiene (assembly)
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    // Si se utiliza dotnet ef se debe indicar el proyecto de inicio (si se llama desde el proyecto que contiene las migraciones (CoronaStatsModel))
    /// dotnet ef migrations add <MigrName> -s|--startup-project "..\CoronaStatsAPI"
    
    // Si se utiliza dotnet ef se debe indicar el proyecto del model (si se llama desde el proyecto que de inicio (CoronaStatsAPI))
    /// dotnet ef migrations add <MigrName> -p|--project "..\CoronaStatsModel"
    
    // Si se utiliza el package Manager (instalando Microsoft.EntityFrameworkCore.Tools)
    // Se selecciona el default project desde el drop down y se ejecuta:
    /// Add-Migration <MigrName>
    
    sqlOptions => sqlOptions.MigrationsAssembly(typeof(CoronaStatsDataContext).Assembly.FullName) 
    );

// Normal way of instantiating the dbcontext for use with SQL server.
// This way can configure more options
//builder.Services.AddDbContext<CoronaStatsDataContext>(options =>
//    options.UseSqlServer(
//        connextionString,
//        sqlOptions => sqlOptions.MigrationsAssembly(typeof(CoronaStatsDataContext).Assembly.FullName)) // Para separar el DbContext en una librería (en este caso CoronaStatsModel) hay que referenciar el assembly
//);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
               );
});

builder.Services.AddHttpClient<IImportDataService, ImportDataService>();
builder.Services.AddScoped<IDataService, DataService>();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
