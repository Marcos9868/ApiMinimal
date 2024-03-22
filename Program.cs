using ApiMinimal.AppServicesExtensions;
using ApiMinimal.ApiEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthetication();
    
var app = builder.Build();

app.CategoryMapEndpoints();
app.AuthenticationMapEndpoints();
app.ProductMapEndpoints();

var environment = app.Environment;

app.UseExceptionHandling(environment)
    .UseSwaggerEndpoints()
    .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
