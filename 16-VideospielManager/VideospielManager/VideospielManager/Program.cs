using Microsoft.EntityFrameworkCore;
using VideospielManager.DataAccess;

var builder = WebApplication.CreateBuilder(args);

/// ///////////////////////////////////////////////////
/// In this section we configure the dependency injection for asp.net core
/// 
// Add services to the container.
// >>> Here we need to register Entity Framework to get db access.
builder.Services.AddDbContext<VideoGameDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Here we add a default policy for CORS (Cross origin resource sharing)
builder.Services.AddCors( options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               );
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


/// ///////////////////////////////////////////////////
var app = builder.Build();

/// ///////////////////////////////////////////////////
/// In this section we add middleware in the data pipeline
/// The data pipeline is where every incoming http request pass through
/// and the middleware is a piece of software that can be used
/// to change the properties (or do something with the "flow" of that incoming request).
/// e.g. it can inspect the request and check if the request has proper username and password,
/// rejecting it or not.
/// 

// Here we need to add CORS to the pipeline
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
