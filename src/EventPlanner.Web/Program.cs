using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventPlanner.Web.Infrastructure;
using EventPlanner.Web.Models.Validators;
using EventPlanner.Web.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews(options =>
    {
        // Отключаем «неявный Required» для non-nullable reference types,
        // чтобы «обязательность» контролировал FluentValidation.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<BearerFromCookieHandler>();

// Typed HttpClient для работы с Locations (аналогично потом сделаешь для событий/билетов)
builder.Services.AddHttpClient<LocationApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!)).
    AddHttpMessageHandler<BearerFromCookieHandler>()
    .Services.AddHttpClient<EventApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .AddHttpMessageHandler<BearerFromCookieHandler>()
    .Services.AddHttpClient<TicketApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .AddHttpMessageHandler<BearerFromCookieHandler>()
    .Services.AddHttpClient<UserApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .AddHttpMessageHandler<BearerFromCookieHandler>()
    .Services.AddHttpClient<BookingApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .AddHttpMessageHandler<BearerFromCookieHandler>()
    .Services.AddHttpClient<AuthApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:AuthApiUrl"]!))
    .AddHttpMessageHandler<BearerFromCookieHandler>();
// убрать 10 хттп клиентов
builder.Services
    .AddFluentValidationAutoValidation()          // серверная валидация
    .AddFluentValidationClientsideAdapters();     // клиентская (jquery unobtrusive)
builder.Services.AddValidatorsFromAssemblyContaining<UpsertEventVmValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,  ValidIssuer   = jwt["Issuer"],
        ValidateAudience = true,ValidAudience = jwt["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        // важно для Razor: как читать имя и роли
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = JwtRegisteredClaimNames.UniqueName // ты кладёшь unique_name
    };

    // читаем токен из куки "Auth"
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            if (ctx.Request.Cookies.TryGetValue("Auth", out var token) && !string.IsNullOrWhiteSpace(token))
                ctx.Token = token;
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

builder.WebHost.UseUrls("http://localhost:5001");
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // <= обязательно перед UseAuthorization
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();