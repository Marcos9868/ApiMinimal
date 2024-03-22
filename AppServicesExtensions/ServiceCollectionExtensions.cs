using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiMinimal.Context;
using ApiMinimal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiMinimal.AppServicesExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwagger();
            return builder;
        }
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiMinimal", Version = "1.0" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization header using the Bearer scheme
                        Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",
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
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
        public static WebApplicationBuilder AddAuthetication(this WebApplicationBuilder builder)
        {
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                    };
                });

            builder.Services.AddAuthorization();

            return builder;
        }
        public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DataContext>(options => 
                options.UseNpgsql(connectionString));

            builder.Services.AddSingleton<ITokenService>(new TokenService());
            return builder;
        }
    }
}