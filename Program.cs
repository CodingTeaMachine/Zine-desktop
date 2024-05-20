using ElectronNET.API;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Zine.App.Database;
using Zine.App.Enums;
using Zine.App.Logger;
using Zine.App.Repositories;
using Zine.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseElectron(args);
builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddElectron();
builder.Services.AddMudServices();

builder.Services.AddSingleton<ILoggerService, SerilogLogger>();

builder.Services.AddDbContextFactory<ZineDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString(ConfigKeys.DbContext)
                  ?? throw new InvalidOperationException("Connection string 'DbContext' not found.")));

builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

builder.Services.AddScoped<IComicRepository, ComicRepository>();
builder.Services.AddScoped<IComicService, ComicService>();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Zine.Components.App>()
    .AddInteractiveServerRenderMode();

await app.StartAsync();

await Electron.WindowManager.CreateWindowAsync();

if (HybridSupport.IsElectronActive)
{
    var window = await Electron.WindowManager.CreateWindowAsync();
    window.OnClosed += () => Electron.App.Quit();
}

app.WaitForShutdown();
