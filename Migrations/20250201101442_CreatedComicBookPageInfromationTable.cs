using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class CreatedComicBookPageInfromationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_ParentGroupId",
                table: "Groups");

            migrationBuilder.CreateTable(
                name: "ComicBookPageInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PageNumberStart = table.Column<int>(type: "INTEGER", nullable: false),
                    PageNumberEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    PageType = table.Column<int>(type: "INTEGER", nullable: false),
                    ComicBookId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicBookPageInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicBookPageInformation_ComicBooks_ComicBookId",
                        column: x => x.ComicBookId,
                        principalTable: "ComicBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookPageInformation_ComicBookId",
                table: "ComicBookPageInformation",
                column: "ComicBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_ParentGroupId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "ComicBookPageInformation");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBooks_Groups_GroupId",
                table: "ComicBooks",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
