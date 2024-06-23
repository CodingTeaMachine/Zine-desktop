using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class CreatedComicBookInfromationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageFileName",
                table: "ComicBooks");

            migrationBuilder.CreateTable(
                name: "ComicBookInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComicBookId = table.Column<int>(type: "INTEGER", nullable: false),
                    PageFormat = table.Column<int>(type: "INTEGER", nullable: false),
                    CoverImage = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicBookInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicBookInformation_ComicBooks_ComicBookId",
                        column: x => x.ComicBookId,
                        principalTable: "ComicBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformation_ComicBookId",
                table: "ComicBookInformation",
                column: "ComicBookId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComicBookInformation");

            migrationBuilder.AddColumn<string>(
                name: "CoverImageFileName",
                table: "ComicBooks",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
