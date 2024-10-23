using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zine.Migrations
{
    /// <inheritdoc />
    public partial class ChanchedComicBookNameToComicBookTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ComicBooks",
                newName: "Title");

            migrationBuilder.AlterColumn<int>(
                name: "ParentGroupId",
                table: "Groups",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ComicBooks",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "ParentGroupId",
                table: "Groups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
