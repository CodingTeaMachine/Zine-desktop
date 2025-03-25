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

            modelBuilder.Entity("ComicBookInformationPerson", b =>
                {
                    b.Property<int>("ComicBookInformationListId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PeopleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ComicBookInformationListId", "PeopleId");

                    b.HasIndex("PeopleId");

                    b.ToTable("ComicBookInformationPerson");
                });

            modelBuilder.Entity("ComicBookInformationPublisher", b =>
                {
                    b.Property<int>("ComicBookInformationListId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PublishersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ComicBookInformationListId", "PublishersId");

                    b.HasIndex("PublishersId");

                    b.ToTable("ComicBookInformationPublisher");
                });

            modelBuilder.Entity("ComicBookInformationTag", b =>
                {
                    b.Property<int>("ComicBookInformationListId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ComicBookInformationListId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("ComicBookInformationTag");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBook.ComicBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileUri")
                        .IsRequired()
                        .HasMaxLength(32767)
                        .HasColumnType("TEXT");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
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

                    b.Property<DateTime?>("LastOpened")
                        .HasColumnType("TEXT");

                    b.Property<string>("SavedCoverImageFileName")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ComicBookId")
                        .IsUnique();

                    b.ToTable("ComicBookInformation");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBookPageInformation.ComicBookPageInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ComicBookId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRead")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsWidthChecked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PageFileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("PageNumberStart")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PageType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ComicBookId");

                    b.ToTable("ComicBookPageInformation");
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

            modelBuilder.Entity("Zine.App.Domain.Person.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Zine.App.Domain.Publisher.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("Zine.App.Domain.Tag.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("ComicBookInformationPerson", b =>
                {
                    b.HasOne("Zine.App.Domain.ComicBookInformation.ComicBookInformation", null)
                        .WithMany()
                        .HasForeignKey("ComicBookInformationListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zine.App.Domain.Person.Person", null)
                        .WithMany()
                        .HasForeignKey("PeopleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComicBookInformationPublisher", b =>
                {
                    b.HasOne("Zine.App.Domain.ComicBookInformation.ComicBookInformation", null)
                        .WithMany()
                        .HasForeignKey("ComicBookInformationListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zine.App.Domain.Publisher.Publisher", null)
                        .WithMany()
                        .HasForeignKey("PublishersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComicBookInformationTag", b =>
                {
                    b.HasOne("Zine.App.Domain.ComicBookInformation.ComicBookInformation", null)
                        .WithMany()
                        .HasForeignKey("ComicBookInformationListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zine.App.Domain.Tag.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBook.ComicBook", b =>
                {
                    b.HasOne("Zine.App.Domain.Group.Group", "Group")
                        .WithMany("ComicBooks")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

            modelBuilder.Entity("Zine.App.Domain.ComicBookPageInformation.ComicBookPageInformation", b =>
                {
                    b.HasOne("Zine.App.Domain.ComicBook.ComicBook", "ComicBook")
                        .WithMany("Pages")
                        .HasForeignKey("ComicBookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ComicBook");
                });

            modelBuilder.Entity("Zine.App.Domain.Group.Group", b =>
                {
                    b.HasOne("Zine.App.Domain.Group.Group", "ParentGroup")
                        .WithMany("ChildGroups")
                        .HasForeignKey("ParentGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ParentGroup");
                });

            modelBuilder.Entity("Zine.App.Domain.ComicBook.ComicBook", b =>
                {
                    b.Navigation("Information")
                        .IsRequired();

                    b.Navigation("Pages");
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
