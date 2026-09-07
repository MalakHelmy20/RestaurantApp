using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Repositories Interfaces
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Application.Features.Restaurants.IRepository;
using MyRestaurantApp.Application.Features.Orders.IRepository;
using MyRestaurantApp.Application.Features.Products.IRepository;
using MyRestaurantApp.Application.Features.Ratings.IRepository;
using MyRestaurantApp.Application.Features.Categories.IRepository;



// Services Interfaces & Implementations
using MyRestaurantApp.Application.Features.Users.Services;
using MyRestaurantApp.Application.Features.Restaurants.Services;
using MyRestaurantApp.Application.Features.Orders.Services;
using MyRestaurantApp.Application.Features.Products.Services;
using MyRestaurantApp.Application.Features.Ratings.Services;

using MyRestaurantApp.Application.Features.Categories.Services;



// Infrastructure Repositories Implementations
using MyRestaurantApp.Infrastructure.Repository.UserRepo;
using MyRestaurantApp.Infrastructure.Repository.RestaurantRepo;
using MyRestaurantApp.Infrastructure.Repository.OrderRepo;
using MyRestaurantApp.Infrastructure.Repository.ProductRepo;
using MyRestaurantApp.Infrastructure.Repository.RatingRepo;
using MyRestaurantApp.Infrastructure.Repository.CategoryRepo;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers service to enable API controller routing
builder.Services.AddControllers();

// Configure Application DbContext with SQL Server using the Connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// Register Users Services & Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Register Restaurants & Categories Services & Repositories
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Register Products Services & Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Register Orders Services & Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Register Ratings Services & Repositories
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();

// Register OpenAPI/Swagger services for API documentation (.NET 9+)
builder.Services.AddOpenApi();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure HTTP request pipeline for Development environment
if (app.Environment.IsDevelopment())
{
    // Expose the OpenAPI JSON endpoint
    app.MapOpenApi();

    // Enable Swagger UI middleware to visualize and test API endpoints
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My Restaurant API v1");
    });
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = context.Response.StatusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : exception?.Message ?? "An error occurred.";

        await context.Response.WriteAsJsonAsync(new { message });
    });
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();