using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusTagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusTagId",
                table: "ComicBookInformation",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StatusTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Color = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTags", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "StatusTags",
                columns: new[] { "Id", "Color", "Name" },
                values: new object[,]
                {
                    { 1, 5, "Finished" },
                    { 2, 4, "Reading" },
                    { 3, 1, "Archived" },
                    { 4, 2, "Want to read" },
                    { 5, 7, "Retired" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformation_StatusTagId",
                table: "ComicBookInformation",
                column: "StatusTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBookInformation_StatusTags_StatusTagId",
                table: "ComicBookInformation",
                column: "StatusTagId",
                principalTable: "StatusTags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComicBookInformation_StatusTags_StatusTagId",
                table: "ComicBookInformation");

            migrationBuilder.DropTable(
                name: "StatusTags");

            migrationBuilder.DropIndex(
                name: "IX_ComicBookInformation_StatusTagId",
                table: "ComicBookInformation");

            migrationBuilder.DropColumn(
                name: "StatusTagId",
                table: "ComicBookInformation");
        }
    }
}
