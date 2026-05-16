using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zent.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueBoardIdTitleColumnIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_columns_title_board_id",
                table: "columns",
                columns: new[] { "title", "board_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_columns_title_board_id",
                table: "columns");
        }
    }
}
