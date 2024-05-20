﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zine.App.Database;

#nullable disable

namespace Zine.Migrations
{
    [DbContext(typeof(ZineDbContext))]
    [Migration("20240517073615_UpdatedSettingModel")]
    partial class UpdatedSettingModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.5");

            modelBuilder.Entity("Zine.App.Model.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("InitialValue")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Key")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
