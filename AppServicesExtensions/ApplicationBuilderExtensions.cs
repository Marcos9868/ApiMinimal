using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMinimal.AppServicesExtensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            return app;
        }
        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(p => 
            {
                p.AllowAnyOrigin();
                p.WithMethods("GET");
                p.AllowAnyHeader();
            });
            return app;
        }
        public static IApplicationBuilder UseSwaggerEndpoints(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {});
            return app;
        }
    }
}