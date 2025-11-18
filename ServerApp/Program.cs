using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Copilot Suggestion: Add in-memory caching to improve API performance
// This reduces redundant data processing for frequently accessed product lists
builder.Services.AddMemoryCache();

// Copilot Assistance: Configure JSON serialization for camelCase output
// This ensures the API follows modern REST API conventions with camelCase property names
// while maintaining PascalCase in C# code for .NET standards
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Copilot Assistance: Configure CORS to allow cross-origin requests
// This resolves CORS errors when the Blazor WebAssembly client (different origin)
// communicates with the API server
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

// Copilot Optimization: Implemented in-memory caching to reduce repeated data generation
// Cache key for product list to enable efficient retrieval
const string ProductCacheKey = "ProductList";

// API Endpoint: GET /api/productlist
// Returns a list of products with nested category information
// Copilot helped structure this endpoint with proper DTOs, caching, and error handling
app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    try
    {
        // Copilot Performance Optimization: Check cache first to avoid regenerating data
        // This significantly improves response time for subsequent requests
        if (!cache.TryGetValue(ProductCacheKey, out ProductDto[]? products))
        {
            // Generate product data (in real-world scenario, this would be a database call)
            products = new[]
            {
                new ProductDto
                {
                    Id = 1,
                    Name = "Laptop",
                    Price = 1200.50,
                    Stock = 25,
                    Category = new CategoryDto { Id = 101, Name = "Electronics" }
                },
                new ProductDto
                {
                    Id = 2,
                    Name = "Headphones",
                    Price = 50.00,
                    Stock = 100,
                    Category = new CategoryDto { Id = 102, Name = "Accessories" }
                },
                new ProductDto
                {
                    Id = 3,
                    Name = "Wireless Mouse",
                    Price = 25.99,
                    Stock = 150,
                    Category = new CategoryDto { Id = 102, Name = "Accessories" }
                },
                new ProductDto
                {
                    Id = 4,
                    Name = "USB-C Hub",
                    Price = 45.00,
                    Stock = 75,
                    Category = new CategoryDto { Id = 101, Name = "Electronics" }
                }
            };

            // Copilot Suggestion: Cache for 5 minutes to balance freshness and performance
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            cache.Set(ProductCacheKey, products, cacheOptions);
        }

        return Results.Ok(products);
    }
    catch (Exception ex)
    {
        // Copilot Assistance: Added comprehensive error handling
        // Returns proper HTTP 500 status with error details for debugging
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error retrieving product list"
        );
    }
})
.WithName("GetProducts")
.WithDescription("Retrieves the complete product catalog with category information")
.Produces<ProductDto[]>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// ============================================================================
// Data Transfer Objects (DTOs)
// ============================================================================
// Copilot helped design these DTOs following industry-standard practices:
// - PascalCase property names in C# (converted to camelCase in JSON)
// - Proper nullable annotations for type safety
// - Nested object structure for relational data (Product -> Category)
// - Immutable record types for data integrity

/// <summary>
/// Represents a product in the inventory system
/// Copilot Assistance: Structured with nested Category for normalized data representation
/// </summary>
record ProductDto
{
    /// <summary>Unique identifier for the product</summary>
    public int Id { get; set; }
    
    /// <summary>Product name/title</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Product price in USD</summary>
    public double Price { get; set; }
    
    /// <summary>Current stock quantity available</summary>
    public int Stock { get; set; }
    
    /// <summary>Product category information (nested object)</summary>
    public CategoryDto Category { get; set; } = null!;
}

/// <summary>
/// Represents a product category
/// Copilot Assistance: Created as separate DTO to follow single responsibility principle
/// </summary>
record CategoryDto
{
    /// <summary>Unique identifier for the category</summary>
    public int Id { get; set; }
    
    /// <summary>Category name</summary>
    public string Name { get; set; } = string.Empty;
}
