using CashCompass.API.Data;
using CashCompass.API.Services;
using Microsoft.EntityFrameworkCore;
// ❌ Replaced using Microsoft.Data.SqlClient;
using Npgsql; // 👈 CRITICAL FIX: Use Npgsql for PostgreSQL
using System.Data;
using CashCompass.API.GraphQL;
using CashCompass.API.GraphQL.Types;
using HotChocolate.Execution.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Application Services (Dependency Injection) ---

// Password Hashing Service 
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>(); 

// Register DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    // 👈 CRITICAL FIX: Use UseNpgsql
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register IDbConnection for Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    // 👈 CRITICAL FIX: Use NpgsqlConnection
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ---------------- CORS Configuration ----------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            // Allow any localhost or 127.0.0.1 port
            return origin.StartsWith("http://localhost") || origin.StartsWith("http://127.0.0.1");
        })
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
// ---------------- End CORS Configuration ----------------

// --- GraphQL HotChocolate Setup ---
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections() 
    .AddFiltering()   
    .AddSorting()     
    .AddType<UserType>()
    .AddType<IncomeSourceType>()
    .AddType<CategoryType>()
    .AddType<AllocationType>()
    .AddType<ExpenseType>()
    // ❌ CRITICAL FIX: Removed TransactionType
    // .AddType<TransactionType>(); 
    .AddType<CashCompass.API.GraphQL.Types.IncomeSourceType>()
    .AddType<CashCompass.API.GraphQL.Types.CategoryType>()
    .AddType<CashCompass.API.GraphQL.Types.AllocationType>()
    .AddType<CashCompass.API.GraphQL.Types.ExpenseType>(); // Ensure all types are added only once

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CashCompass API V1");
        options.RoutePrefix = string.Empty; 
    });
    // This assumes you have the NuGet package HotChocolate.AspNetCore.Voyager
    app.UseGraphQLVoyager("/graphql-voyager"); 
}

app.UseHttpsRedirection();

// Enable CORS (Applies the Default Policy)
app.UseCors(); 

app.UseAuthorization();

app.MapControllers();
// 💡 GraphQL Endpoint Mapping
app.MapGraphQL("/graphql"); 

app.Run();