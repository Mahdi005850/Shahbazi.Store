using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<CartServices>();
builder.Services.AddScoped<OrderServices>();
builder.Services.AddScoped<UserServices>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ShahbaziStoreDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Shahbazi Store API Is running !!");

app.MapProductEndpoints();

app.MapUserEndpoints();

app.MapCartEndpoints();

app.MapOrderEndpoints();

app.Run();