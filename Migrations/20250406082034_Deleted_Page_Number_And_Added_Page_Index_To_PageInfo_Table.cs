using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class Deleted_Page_Number_And_Added_Page_Index_To_PageInfo_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageNumberStart",
                table: "ComicBookPageInformation",
                newName: "Index");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Index",
                table: "ComicBookPageInformation",
                newName: "PageNumberStart");
        }
    }
}
