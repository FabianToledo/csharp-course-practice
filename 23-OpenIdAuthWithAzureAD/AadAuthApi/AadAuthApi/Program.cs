using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// We add protection to the API using Microsoft identity platform (Azure AD)
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

// We add authentication and authorization services to our API
// This is not part of Azure AD,
// if we want auth services we have to add these no matter the Auth server.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Add CORS middleware allowing all for demo purposes.
app.UseCors(policy => 
    policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );

app.UseHttpsRedirection();

// Add Authentication middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
