using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IntelligentConnector.Core.Interfaces;
using IntelligentConnector.Core.Models;
using IntelligentConnector.Data;
using Microsoft.EntityFrameworkCore;
using IntelligentConnector.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IPublicApiConnector, PublicApiConnector>(client =>
{
    client.BaseAddress = new Uri("https://api.restful-api.dev/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapSyncEndpoints();


app.Run();

