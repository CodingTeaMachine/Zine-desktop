using ElectronNET.API;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Zine.App.Database;
using Zine.App.Domain.ComicBook;
using Zine.App.Domain.Group;
using Zine.App.Domain.Settings;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Logger;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseElectron(args);
builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddElectron();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
});

builder.Services.AddSingleton<ILoggerService, SerilogLogger>();

builder.Services.AddDbContextFactory<ZineDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString(ConfigKeys.DbContext)
                  ?? throw new InvalidOperationException("Connection string 'DbContext' not found.")));

builder.Services.AddScoped<ISettingsRepository, SettingsContextFactory>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

builder.Services.AddScoped<IComicBookRepository, ComicBookRepository>();
builder.Services.AddScoped<IComicBookService, ComicBookService>();
builder.Services.AddScoped<IComicBookImportService, ComicBookImportService>();

builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();

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

ConfigurationHelper.Initialize(builder.Configuration);

ComicBookService.CleanReadingDirectory();

app.WaitForShutdown();
