﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zine.App.Database;

#nullable disable

namespace Zine.Migrations
{
    [DbContext(typeof(ZineDbContext))]
    partial class ZineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.5");

            modelBuilder.Entity("Zine.App.Domain.ComicBook.ComicBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileUri")
                        .IsRequired()
                        .HasMaxLength(32767)
                        .HasColumnType("TEXT");

                    b.Property<int?>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("ComicBooks");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBookInformation.ComicBookInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ComicBookId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CoverImage")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("PageNamingFormat")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ComicBookId")
                        .IsUnique();

                    b.ToTable("ComicBookInformation");
                });

            modelBuilder.Entity("Zine.App.Domain.Group.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentGroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Zine.App.Domain.Settings.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("InitialValue")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBook.ComicBook", b =>
                {
                    b.HasOne("Zine.App.Domain.Group.Group", "Group")
                        .WithMany("ComicBooks")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientNoAction);

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBookInformation.ComicBookInformation", b =>
                {
                    b.HasOne("Zine.App.Domain.ComicBook.ComicBook", "ComicBook")
                        .WithOne("Information")
                        .HasForeignKey("Zine.App.Domain.ComicBookInformation.ComicBookInformation", "ComicBookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ComicBook");
                });

            modelBuilder.Entity("Zine.App.Domain.Group.Group", b =>
                {
                    b.HasOne("Zine.App.Domain.Group.Group", "ParentGroup")
                        .WithMany("ChildGroups")
                        .HasForeignKey("ParentGroupId")
                        .OnDelete(DeleteBehavior.ClientNoAction);

                    b.Navigation("ParentGroup");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBook.ComicBook", b =>
                {
                    b.Navigation("Information")
                        .IsRequired();
                });

            modelBuilder.Entity("Zine.App.Domain.Group.Group", b =>
                {
                    b.Navigation("ChildGroups");

                    b.Navigation("ComicBooks");
                });
#pragma warning restore 612, 618
        }
    }
}
