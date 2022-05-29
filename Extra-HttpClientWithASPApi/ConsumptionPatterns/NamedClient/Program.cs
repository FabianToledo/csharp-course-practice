using NamedClient.NamedClientServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/////////////////////////////////////////////////////////////////////////////////////////
// Register the Named Client and configure it.
// In injected class the created HttpClient with factory.CreateClient will have this configuration
builder.Services.AddHttpClient("NamedClient", httpClient => 
    httpClient.BaseAddress = new Uri("https://api.chucknorris.io")
);

// Register a service in DI container
builder.Services.AddScoped<INamedClientService, NamedClientService>();
/////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

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
