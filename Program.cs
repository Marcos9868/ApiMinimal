using ApiMinimal.Context;
using ApiMinimal.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapPost("/categories", async (Category category, DataContext context) => 
{
    context.Categories.Add(category);
    await context.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
});

app.MapGet("/categories", async (DataContext context) => 
    await context.Categories.ToListAsync());

app.MapGet("/categories/{id}", async (int id, DataContext context) => 
{
    return await context.Categories.FindAsync(id)
        is Category category 
            ?   Results.Ok(category)
            :   Results.NotFound();
});

app.MapPut("/categories/{id:int}", async (int id, Category category, DataContext context) => 
{
    var categoryDb = await context.Categories.FindAsync(id);
    if (categoryDb is null) return Results.NotFound();

    categoryDb.Name = category.Name;
    categoryDb.Description = category.Description;

    await context.SaveChangesAsync();
    return Results.Ok(categoryDb);
});

app.MapDelete("/categories/{id}", async (int id, DataContext context) => 
{
    var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (category is null)
        return Results.NotFound();
        
    context.Categories.Remove(category);
    await context.SaveChangesAsync();
    return Results.Ok("Category removed sucessfully");
});

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();


app.UseSwaggerUI();

app.Run();
