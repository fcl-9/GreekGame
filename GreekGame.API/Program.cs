using GreekGame.API.Application;
using GreekGame.API.BackgroundServices;
using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// DB (use PostgreSQL or SQL Server)
builder.Services.AddDbContext<GameDbContext>(options => options.UseSqlite("GameDb"));

// Services
builder.Services.AddScoped<ICityService, CityService>();

// Game loop
builder.Services.AddHostedService<GameLoopService>();

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
