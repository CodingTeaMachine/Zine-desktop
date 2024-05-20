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
}
