using EventPlanner.Web.Infrastructure;
using EventPlanner.Web.Models.Validators;
using EventPlanner.Web.Services;
using FluentValidation;
using FluentValidation.AspNetCore;

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

builder.Services
    .AddFluentValidationAutoValidation()          // серверная валидация
    .AddFluentValidationClientsideAdapters();     // клиентская (jquery unobtrusive)

builder.Services.AddValidatorsFromAssemblyContaining<UpsertEventVmValidator>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();