using ElectronNET.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MudBlazor;
using MudBlazor.Services;
using Zine.App.Database;
using Zine.App.Domain.ComicBook;
using Zine.App.Domain.ComicBook.Import;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Domain.Group;
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

builder.Services.AddScoped<IComicBookService, ComicBookService>();
builder.Services.AddScoped<IComicBookImportService, ComicBookImportService>();

builder.Services.AddScoped<IComicBookInformationService, ComicBookInformationService>();

builder.Services.AddScoped<IComicBookPageInformationService, ComicBookPageInformationService>();

builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddScoped(typeof(GenericRepository<>));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapRazorComponents<Zine.Components.App>()
    .AddInteractiveServerRenderMode();

if (!Directory.Exists(DataPath.ComicBookReadingDirectory))
{
    Directory.CreateDirectory(DataPath.ComicBookReadingDirectory);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(DataPath.ComicBookReadingDirectory),
    RequestPath = "/images/Reading", // Maps the directory to this URL path
    ServeUnknownFileTypes = true
});


app.UseStaticFiles();

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
