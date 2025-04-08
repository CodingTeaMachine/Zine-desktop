using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class Create_Publishers_And_Tags_Tabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComicBookInformationPublisher",
                columns: table => new
                {
                    ComicBookInformationListId = table.Column<int>(type: "INTEGER", nullable: false),
                    PublishersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicBookInformationPublisher", x => new { x.ComicBookInformationListId, x.PublishersId });
                    table.ForeignKey(
                        name: "FK_ComicBookInformationPublisher_ComicBookInformation_ComicBookInformationListId",
                        column: x => x.ComicBookInformationListId,
                        principalTable: "ComicBookInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicBookInformationPublisher_Publishers_PublishersId",
                        column: x => x.PublishersId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicBookInformationTag",
                columns: table => new
                {
                    ComicBookInformationListId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicBookInformationTag", x => new { x.ComicBookInformationListId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ComicBookInformationTag_ComicBookInformation_ComicBookInformationListId",
                        column: x => x.ComicBookInformationListId,
                        principalTable: "ComicBookInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicBookInformationTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformationPublisher_PublishersId",
                table: "ComicBookInformationPublisher",
                column: "PublishersId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicBookInformationTag_TagsId",
                table: "ComicBookInformationTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComicBookInformationPublisher");

            migrationBuilder.DropTable(
                name: "ComicBookInformationTag");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
