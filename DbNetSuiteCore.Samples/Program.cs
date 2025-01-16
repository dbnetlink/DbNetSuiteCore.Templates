using DbNetSuiteCore.Middleware;
using DbNetSuiteCore.Samples.Helpers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDbNetSuiteCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGet("/superstore", () =>
    FileHelper.GetJson("/data/json/superstore.json", builder.Environment));


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseDbNetSuiteCore();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");


app.Run();
