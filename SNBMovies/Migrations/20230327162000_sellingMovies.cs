using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNBMovies.Migrations
{
    /// <inheritdoc />
    public partial class sellingMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "ShoppingCartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPurchased",
                table: "ShoppingCartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "IsPurchased",
                table: "ShoppingCartItems");
        }
    }
}
