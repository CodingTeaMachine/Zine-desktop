using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class RenameFileNameToFileUriInCB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "ComicBooks",
                newName: "FileUri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileUri",
                table: "ComicBooks",
                newName: "FileName");
        }
    }
}
