using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class AddedPageWidthCheckedColToPageInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWidthChecked",
                table: "ComicBookPageInformation",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWidthChecked",
                table: "ComicBookPageInformation");
        }
    }
}
