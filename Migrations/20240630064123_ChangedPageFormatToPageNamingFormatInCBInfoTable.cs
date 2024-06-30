using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPageFormatToPageNamingFormatInCBInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageFormat",
                table: "ComicBookInformation",
                newName: "PageNamingFormat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageNamingFormat",
                table: "ComicBookInformation",
                newName: "PageFormat");
        }
    }
}
