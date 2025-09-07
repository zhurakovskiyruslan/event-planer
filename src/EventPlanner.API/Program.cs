using EventPlanner.Data;
using EventPlanner.Infrastructure;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.DependencyInjection;
using System.Text.Json.Serialization;
using EventPlanner.API.Middlewares;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Подключаем DbContext к PostgreSQL
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Подключаем репозитории через наше расширение
builder.Services.AddInfrastructure();

// Добавляем контроллеры
builder.Services.AddControllers();

// Подключаем FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<EventPlanner.Application.Common.Validation.EventValidator>();

// Подключаем Swagger (удобно для теста API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
var app = builder.Build();

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();