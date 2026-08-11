using RushTodo.Api.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("RushTodo")
    ?? throw new InvalidOperationException("Connection string 'RushTodo' is not configured.");
builder.Services.AddRushTodoApi(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
