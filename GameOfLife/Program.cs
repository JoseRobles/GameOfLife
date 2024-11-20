using Microsoft.EntityFrameworkCore;
using GameOfLife.Services;
using GameOfLife.Models.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<DefaultStepsConfig>(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
