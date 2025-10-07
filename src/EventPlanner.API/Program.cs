using System.Security.Claims;
using System.Text;
using EventPlanner.Data;
using EventPlanner.Infrastructure;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.DependencyInjection;
using System.Text.Json.Serialization;
using EventPlanner.API.Middlewares;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidIssuer = jwt["Issuer"],
            ValidateAudience = true, ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true, IssuerSigningKey = key,
            ValidateLifetime = true, ClockSkew = TimeSpan.Zero
        };
        o.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
        o.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    });
builder.Services.Configure<IdentityOptions>(o => { o.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier; });

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<EventPlanner.Application.Common.Validation.EventValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(opt => { opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin")); });

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
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();