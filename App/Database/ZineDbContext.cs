using Microsoft.EntityFrameworkCore;
using Zine.App.Enums;
using Zine.App.Model.DB;

namespace Zine.App.Database;

public class ZineDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Setting> Settings { get; set; }

    private string DbPath { get; } = Path.Join(
        Directory.GetCurrentDirectory(),
        configuration.GetConnectionString(ConfigKeys.DbContext)
    );

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Setting>().HasData(new Setting{ Id = 1, Key = SettingsKeys.ComicBookPath, InitialValue = ""});
    }
}
