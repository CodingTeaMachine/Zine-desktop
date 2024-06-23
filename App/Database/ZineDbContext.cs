using Microsoft.EntityFrameworkCore;
using Zine.App.Domain.ComicBook;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.Group;
using Zine.App.Domain.Settings;
using Zine.App.Enums;

namespace Zine.App.Database;

public class ZineDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Setting> Settings { get; set; }
    public DbSet<ComicBook> ComicBooks { get; set; }
    public DbSet<ComicBookInformation> ComicBookInformation { get; set; }
    public DbSet<Group> Groups { get; set; }

    private string DbPath { get; } = Path.Join(
        Directory.GetCurrentDirectory(),
        configuration.GetConnectionString(ConfigKeys.DbContext)
    );

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Group referencing groups
        modelBuilder.Entity<Group>()
            .HasOne(g => g.ParentGroup)
            .WithMany(g => g.ChildGroups)
            .HasForeignKey(g => g.ParentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // ComicBook group relationship
        modelBuilder.Entity<ComicBook>()
            .HasOne(cb => cb.Group)
            .WithMany(g => g.ComicBooks)
            .HasForeignKey(cb => cb.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // ComicBook information relationship
        modelBuilder.Entity<ComicBook>()
            .HasOne(cb => cb.Information)
            .WithOne(info => info.ComicBook)
            .HasForeignKey<ComicBookInformation>(info => info.ComicBookId);


        base.OnModelCreating(modelBuilder);
    }
}
