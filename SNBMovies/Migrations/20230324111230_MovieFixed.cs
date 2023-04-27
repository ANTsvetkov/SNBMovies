using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNBMovies.Migrations
{
    /// <inheritdoc />
    public partial class MovieFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieFile",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieFile",
                table: "Movies");
        }
    }
}
