using RushTodo.Api.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("RushTodo")
    ?? throw new InvalidOperationException("Connection string 'RushTodo' is not configured.");
builder.Services.AddRushTodoApi(connectionString);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
