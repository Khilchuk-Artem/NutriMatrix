using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FoodRecords.Api.Migrations
{
    /// <inheritdoc />
    public partial class MinorMealChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientSnapshot");

            migrationBuilder.DropTable(
                name: "RecipeRecords");

            migrationBuilder.CreateTable(
                name: "MealRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateEaten = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ServingsEaten = table.Column<float>(type: "real", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealIngredientSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodMeasureId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    MealRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealIngredientSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealIngredientSnapshots_MealRecords_MealRecordId",
                        column: x => x.MealRecordId,
                        principalTable: "MealRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealIngredientSnapshots_MealRecordId",
                table: "MealIngredientSnapshots",
                column: "MealRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealIngredientSnapshots");

            migrationBuilder.DropTable(
                name: "MealRecords");

            migrationBuilder.CreateTable(
                name: "RecipeRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateEaten = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    Amount = table.Column<float>(type: "real", nullable: false),
                    FoodMeasureId = table.Column<long>(type: "bigint", nullable: false),
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
    }
}
