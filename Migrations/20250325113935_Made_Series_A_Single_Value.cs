using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class Made_Series_A_Single_Value : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComicBookInformationSeries");

            migrationBuilder.AddColumn<int>(
                name: "SeriesId",
                table: "ComicBookInformation",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformation_SeriesId",
                table: "ComicBookInformation",
                column: "SeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComicBookInformation_Series_SeriesId",
                table: "ComicBookInformation",
                column: "SeriesId",
                principalTable: "Series",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComicBookInformation_Series_SeriesId",
                table: "ComicBookInformation");

            migrationBuilder.DropIndex(
                name: "IX_ComicBookInformation_SeriesId",
                table: "ComicBookInformation");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                table: "ComicBookInformation");

            migrationBuilder.CreateTable(
                name: "ComicBookInformationSeries",
                columns: table => new
                {
                    ComicBookInformationListId = table.Column<int>(type: "INTEGER", nullable: false),
                    SeriesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicBookInformationSeries", x => new { x.ComicBookInformationListId, x.SeriesId });
                    table.ForeignKey(
                        name: "FK_ComicBookInformationSeries_ComicBookInformation_ComicBookInformationListId",
                        column: x => x.ComicBookInformationListId,
                        principalTable: "ComicBookInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicBookInformationSeries_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformationSeries_SeriesId",
                table: "ComicBookInformationSeries",
                column: "SeriesId");
        }
    }
}
