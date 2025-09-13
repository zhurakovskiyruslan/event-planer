using System.Text;
using EventPlanner.Data;
using EventPlanner.Infrastructure;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.DependencyInjection;
using System.Text.Json.Serialization;
using EventPlanner.API.Middlewares;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Подключаем DbContext к PostgreSQL
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,   ValidIssuer   = jwt["Issuer"],
            ValidateAudience = true, ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true, IssuerSigningKey = key,
            ValidateLifetime = true, ClockSkew = TimeSpan.Zero
        };
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
//builder.Services.AddHttpLogging(o => { o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders; });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

builder.Services.AddApplication();
var app = builder.Build();

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
//app.UseHttpLogging();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();