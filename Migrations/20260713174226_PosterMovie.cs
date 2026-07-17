using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myApp.Migrations
{
    /// <inheritdoc />
    public partial class PosterMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterFileName",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterFileName",
                table: "Movies");
        }
    }
}
