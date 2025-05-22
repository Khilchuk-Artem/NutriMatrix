using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FoodRecords.Api.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodPlans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FoodMeasureId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecurringDaysRaw = table.Column<string>(type: "text", nullable: true),
                    RequireConfirmationOnAdd = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateEaten = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FoodMeasureId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateEaten = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientSnapshot",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodMeasureId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    RecipeRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientSnapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientSnapshot_RecipeRecords_RecipeRecordId",
                        column: x => x.RecipeRecordId,
                        principalTable: "RecipeRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientSnapshot_RecipeRecordId",
                table: "IngredientSnapshot",
                column: "RecipeRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodPlans");

            migrationBuilder.DropTable(
                name: "FoodRecords");

            migrationBuilder.DropTable(
                name: "IngredientSnapshot");

            migrationBuilder.DropTable(
                name: "RecipeRecords");
        }
    }
}
