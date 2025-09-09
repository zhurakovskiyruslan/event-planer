using EventPlanner.Web.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Typed HttpClient для работы с Locations (аналогично потом сделаешь для событий/билетов)
builder.Services.AddHttpClient<LocationApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .Services.AddHttpClient<EventApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .Services.AddHttpClient<TicketApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .Services.AddHttpClient<UserApiClient>(c => 
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!));

builder.Services.AddValidatorsFromAssemblyContaining<EventPlanner.Web.Models.Validators.UpsertEventVmValidator>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();