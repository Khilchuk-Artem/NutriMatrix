using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodCatalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddServing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalServings",
                table: "Meals",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalServings",
                table: "Meals");
        }
    }
}
