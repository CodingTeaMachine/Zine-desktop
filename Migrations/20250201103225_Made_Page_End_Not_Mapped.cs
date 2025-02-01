using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class Made_Page_End_Not_Mapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNumberEnd",
                table: "ComicBookPageInformation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageNumberEnd",
                table: "ComicBookPageInformation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
