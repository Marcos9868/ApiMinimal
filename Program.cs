using System.Text;
using ApiMinimal.Context;
using ApiMinimal.Models;
using ApiMinimal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiMinimal", Version = "1.0" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer + 123abcdef"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<DataContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
// Login
app.MapPost("/login", [AllowAnonymous] (User user, ITokenService TokenService) => 
{
    if (user is null) return Results.BadRequest("Invalid Login");
    if (
        user.Username == "Marcos" &&
        user.Password == "testeApi123"
    )
    {
        var tokenString = TokenService.GenerateToken(
            app.Configuration["Jwt:SecretKey"],
            app.Configuration["Jwt:Issuer"],
            app.Configuration["Jwt:Audience"],
            user);
        return Results.Ok(new { token = tokenString });
    }
    else
    {
        return Results.BadRequest("Invalid login");
    }
})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("Login")
.WithTags("Auth");

// Categories
app.MapPost("/categories", async (Category category, DataContext context) => 
{
    context.Categories.Add(category);
    await context.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
})
.WithName("CreateCategory")
.WithTags("Categories")
.RequireAuthorization();

app.MapGet("/categories", async (DataContext context) => 
    await context.Categories.ToListAsync())
    .WithName("ListCategories")
    .WithTags("Categories");

app.MapGet("/categories/{id}", async (int id, DataContext context) => 
{
    return await context.Categories.FindAsync(id)
        is Category category 
            ?   Results.Ok(category)
            :   Results.NotFound();
})
.WithName("ListCategory")
.WithTags("Categories");

app.MapPut("/categories/{id:int}", async (int id, Category category, DataContext context) => 
{
    var categoryDb = await context.Categories.FindAsync(id);
    if (categoryDb is null) return Results.NotFound();

    categoryDb.Name = category.Name;
    categoryDb.Description = category.Description;

    await context.SaveChangesAsync();
    return Results.Ok(categoryDb);
})
.WithName("UpdateCategory")
.WithTags("Categories");

app.MapDelete("/categories/{id}", async (int id, DataContext context) => 
{
    var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (category is null)
        return Results.NotFound();
        
    context.Categories.Remove(category);
    await context.SaveChangesAsync();
    return Results.Ok("Category removed sucessfully");
})
.WithName("RemoveCategory")
.WithTags("Categories");

app.MapGet("/productCategory", async (DataContext context) => 
{
    return await context.Categories
        .Include(c => c.Products)
        .ToListAsync();
})
.Produces<List<Category>>(StatusCodes.Status200OK)
.WithName("ListProductsByCategory")
.WithTags("Categories");

// Products
app.MapPost("/products", async (Product product, DataContext context) => 
{
    context.Products.Add(product);
    await context.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
})
.Produces<Product>(StatusCodes.Status201Created)
.WithName("CreateProduct")
.WithTags("Products");

app.MapGet("/products", async (DataContext context) => 
    await context.Products.ToListAsync())
    .Produces<Product>(StatusCodes.Status200OK)
    .WithName("ListProducts")
    .WithTags("Products");

app.MapGet("/products/{id}", async (int id, DataContext context) => 
{
    return await context.Products.FindAsync(id)
        is Product product 
            ?   Results.Ok(product)
            :   Results.NotFound();
})
.Produces<Product>(StatusCodes.Status200OK)
.WithName("ListProduct")
.WithTags("Products");

app.MapGet("/products/name/{crit}", (string crit, DataContext context) => 
{
    var selectedProducts = context.Products.Where(p => p.Name
        .ToLower().Contains(crit.ToLower()))
        .ToList();

    return selectedProducts.Count > 0
        ? Results.Ok(selectedProducts)
        : Results.NotFound(Array.Empty<Product>());
})
.WithName("ListProductByCrit")
.WithTags("Products");

app.MapGet("/productsPerPage", async (int pageNumber, int pageSize, DataContext context) => 
{
    return await context.Products
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
})
.Produces<List<Product>>(StatusCodes.Status200OK)
.WithName("ListProductsByPage")
.WithTags("Products");


app.MapPut("/products/{id:int}", async (int id, Product product, DataContext context) => 
{
    var productDb = await context.Products.FindAsync(id);
    if (productDb is null) return Results.NotFound();

    productDb.Name = product.Name;
    productDb.Description = product.Description;

    await context.SaveChangesAsync();
    return Results.Ok(productDb);
})
.Produces<Product>(StatusCodes.Status200OK)
.WithName("UpdateProduct")
.WithTags("Products");

app.MapDelete("/products/{id}", async (int id, DataContext context) => 
{
    var product = await context.Products.FirstOrDefaultAsync(c => c.Id == id);
    if (product is null)
        return Results.NotFound();
        
    context.Products.Remove(product);
    await context.SaveChangesAsync();
    return Results.Ok("Product removed sucessfully");
})
.Produces<Product>(StatusCodes.Status200OK)
.WithName("RemoveProduct")
.WithTags("Products");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
    
app.UseAuthentication();
app.UseAuthorization();

app.Run();
