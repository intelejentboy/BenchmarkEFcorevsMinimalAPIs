using BechmarkADOvsEntity.Benchmarks;
using BechmarkADOvsEntity.Models;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure Minimal API
app.MapPost("/minimal/todo", async (TodoItem todo, TodoContext db) =>
{
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/minimal/todo/{todo.Id}", todo);
});

app.MapGet("/minimal/todo", async (TodoContext db) =>
{
    return await db.TodoItems.ToListAsync();
});

app.MapGet("/minimal/todo/{id}", async (int id, TodoContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

// Use the connection string for EF Core CRUD operations
app.MapPost("/ef/todo", async (TodoItem todo, TodoContext db) =>
{
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/ef/todo/{todo.Id}", todo);
});

app.MapGet("/ef/todo", async (TodoContext db) =>
{
    return await db.TodoItems.ToListAsync();
});

app.MapGet("/ef/todo/{id}", async (int id, TodoContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});
if (args.Contains("--benchmark"))
{
    BenchmarkRunner.Run<TodoBenchmarks>();
    return; // Exit the application after running benchmarks
}


app.Run();

