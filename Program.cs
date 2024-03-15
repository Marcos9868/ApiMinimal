using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options => 
    options.UseInMemoryDatabase("Clients"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();

app.MapGet("/clients", async (DataContext context) => 
    await context.Clients.ToListAsync());

app.MapGet("/clients/{id}", async (int id, DataContext context) => 
    await context.Clients.FirstOrDefaultAsync(c => c.Id == id));

app.MapPost("/clients", async (Client client, DataContext context) => 
{
    context.Clients.Add(client);
    await context.SaveChangesAsync();
    return client;
});

app.MapPut("/clients/{id}", async (int id, Client client, DataContext context) => 
{
    context.Entry(client).State = EntityState.Modified;
    await context.SaveChangesAsync();
    return client;
});

app.MapDelete("/clients/{id}", async (int id, DataContext context) => 
{
    var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
    if (client is null)
        throw new Exception("Unable to find client");
        
    context.Clients.Remove(client);
    await context.SaveChangesAsync();
    return true;
});

app.UseSwaggerUI();

app.Run();

public class Client
{
    public int Id { get; set; } = 0;
    public string Nome { get; set; } = string.Empty;
}

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {}

    public DbSet<Client> Clients { get; set; }
}
