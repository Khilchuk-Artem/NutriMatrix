using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationService.Api.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToTotalServings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Recipes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Recipes");
        }
    }
}
