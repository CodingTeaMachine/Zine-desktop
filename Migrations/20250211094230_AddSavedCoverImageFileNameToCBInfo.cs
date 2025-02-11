using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedCoverImageFileNameToCBInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "ComicBookInformation");

            migrationBuilder.DropColumn(
                name: "NumberOfPages",
                table: "ComicBookInformation");

            migrationBuilder.DropColumn(
                name: "PageNamingFormat",
                table: "ComicBookInformation");

            migrationBuilder.AddColumn<string>(
                name: "SavedCoverImageFileName",
                table: "ComicBookInformation",
                type: "TEXT",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedCoverImageFileName",
                table: "ComicBookInformation");

            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "ComicBookInformation",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPages",
                table: "ComicBookInformation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PageNamingFormat",
                table: "ComicBookInformation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
