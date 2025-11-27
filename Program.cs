using CashCompass.API.Data;
using CashCompass.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using CashCompass.API.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Dependency Injection
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// ✅ GraphQL setup
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

// ✅ Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ✅ Register IDbConnection for Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CashCompass API V1");
        options.RoutePrefix = string.Empty; // Swagger available at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// ✅ Map GraphQL endpoint
app.MapGraphQL();

app.Run();
