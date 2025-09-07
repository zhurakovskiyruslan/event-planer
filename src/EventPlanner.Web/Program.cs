using EventPlanner.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Typed HttpClient для работы с Locations (аналогично потом сделаешь для событий/билетов)
builder.Services.AddHttpClient<LocationsApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .Services.AddHttpClient<EventsApiClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!));

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