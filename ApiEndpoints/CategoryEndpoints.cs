using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMinimal.Context;
using ApiMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMinimal.ApiEndpoints
{
    public static class CategoryEndpoints
    {
        public static void CategoryMapEndpoints(this WebApplication app)
        {
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
                .WithTags("Categories")
                .RequireAuthorization();

            app.MapGet("/categories/{id}", async (int id, DataContext context) =>
            {
                return await context.Categories.FindAsync(id)
                    is Category category
                        ? Results.Ok(category)
                        : Results.NotFound();
            })
            .WithName("ListCategory")
            .WithTags("Categories")
            .RequireAuthorization();

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
            .WithTags("Categories")
            .RequireAuthorization();

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
            .WithTags("Categories")
            .RequireAuthorization();

            app.MapGet("/productCategory", async (DataContext context) =>
            {
                return await context.Categories
                    .Include(c => c.Products)
                    .ToListAsync();
            })
            .Produces<List<Category>>(StatusCodes.Status200OK)
            .WithName("ListProductsByCategory")
            .WithTags("Categories")
            .RequireAuthorization();
        }
    }
}