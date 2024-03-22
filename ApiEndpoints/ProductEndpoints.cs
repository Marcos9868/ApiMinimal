using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMinimal.Context;
using ApiMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMinimal.ApiEndpoints
{
    public static class ProductEndpoints
    {
        public static void ProductMapEndpoints(this WebApplication app)
        {
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
                        ? Results.Ok(product)
                        : Results.NotFound();
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
        }
    }
}