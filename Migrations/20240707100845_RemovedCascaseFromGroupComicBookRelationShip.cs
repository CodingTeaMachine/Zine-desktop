using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCascaseFromGroupComicBookRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
