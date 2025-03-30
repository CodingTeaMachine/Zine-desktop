using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class Drop_Issues_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_ComicBookInformation_Issues_IssueId",
                table: "ComicBookInformation");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_ComicBookInformation_IssueId",
                table: "ComicBookInformation");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "ComicBookInformation");

            migrationBuilder.AddColumn<string>(
                name: "Issue",
                table: "ComicBookInformation",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issue",
                table: "ComicBookInformation");

            migrationBuilder.AddColumn<int>(
                name: "IssueId",
                table: "ComicBookInformation",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformation_IssueId",
                table: "ComicBookInformation",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_SeriesId",
                table: "Issues",
                column: "SeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBookInformation_Issues_IssueId",
                table: "ComicBookInformation",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");
        }
    }
}
