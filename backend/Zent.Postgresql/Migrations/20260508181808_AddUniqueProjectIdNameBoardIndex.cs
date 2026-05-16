using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zent.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueProjectIdNameBoardIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_boards_project_id_name",
                table: "boards",
                columns: new[] { "project_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_boards_project_id_name",
                table: "boards");
        }
    }
}
