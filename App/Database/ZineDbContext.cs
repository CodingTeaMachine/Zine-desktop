using Microsoft.EntityFrameworkCore;
using Zine.App.Domain.ComicBook;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Domain.Group;
using Zine.App.Domain.Person;
using Zine.App.Domain.Publisher;
using Zine.App.Domain.Series;
using Zine.App.Domain.StatusTag;
using Zine.App.Domain.Tag;
using Zine.App.Enums;

namespace Zine.App.Database;

public class ZineDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<ComicBook> ComicBooks { get; init; }
    public DbSet<ComicBookInformation> ComicBookInformation { get; init; }
    public DbSet<ComicBookPageInformation> ComicBookPageInformation { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<Person> People { get; init; }
    public DbSet<Publisher> Publishers { get; init; }
    public DbSet<Series> Series { get; init; }
    public DbSet<Tag> Tags { get; init; }
    public DbSet<StatusTag> StatusTags { get; init; }

    private string DbPath { get; } = Path.Join(
        Directory.GetCurrentDirectory(),
        configuration.GetConnectionString(ConfigKeys.DbContext)
    );

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Group - Group relationship
        modelBuilder.Entity<Group>()
            .HasOne(g => g.ParentGroup)
            .WithMany(g => g.ChildGroups)
            .HasForeignKey(g => g.ParentGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // ComicBook - Group relationship
        modelBuilder.Entity<ComicBook>()
            .HasOne(cb => cb.Group)
            .WithMany(g => g.ComicBooks)
            .HasForeignKey(cb => cb.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // ComicBook - ComicBookInformation relationship
        modelBuilder.Entity<ComicBook>()
            .HasOne(cb => cb.Information)
            .WithOne(info => info.ComicBook)
            .HasForeignKey<ComicBookInformation>(info => info.ComicBookId)
            .OnDelete(DeleteBehavior.Cascade);

        // ComicBook - ComicBookPageInformation relationship
        modelBuilder.Entity<ComicBook>()
            .HasMany(cb => cb.Pages)
            .WithOne(info => info.ComicBook)
            .HasForeignKey(info => info.ComicBookId)
            .OnDelete(DeleteBehavior.Cascade);

        // ComicBookInformation - Person relationship
        modelBuilder.Entity<ComicBookInformation>()
            .HasMany(i => i.People)
            .WithMany(p => p.ComicBookInformationList);

        //ComicBookInformation - Publisher relationship
        modelBuilder.Entity<ComicBookInformation>()
            .HasMany(i => i.Publishers)
            .WithMany(p => p.ComicBookInformationList);

        //ComicBookInformation - Tag relationship
        modelBuilder.Entity<ComicBookInformation>()
            .HasMany(i => i.Tags)
            .WithMany(t => t.ComicBookInformationList);

        //ComicBookInformation - Status Tag relationship
        modelBuilder.Entity<ComicBookInformation>()
            .HasOne(cbi => cbi.StatusTag)
            .WithMany(t => t.ComicBookInformationList);

        //ComicBookInformation - Series relationship
        modelBuilder.Entity<ComicBookInformation>()
            .HasOne(i => i.Series)
            .WithMany(s => s.ComicBookInformationList);


        modelBuilder.Entity<StatusTag>()
            .HasData(BaseStatusTags.StatusTags);

        base.OnModelCreating(modelBuilder);
    }
}
