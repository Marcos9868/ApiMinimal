using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMinimal.Models;
using ApiMinimal.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiMinimal.ApiEndpoints
{
    public static class AuthenticationEndpoints
    {
        public static void AuthenticationMapEndpoints(this WebApplication app)
        {
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
        }
    }
}