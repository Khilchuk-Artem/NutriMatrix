using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FoodRecords.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                    ConsumableId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RequiresConfirmation = table.Column<bool>(type: "boolean", nullable: false),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    RunAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CronExpression = table.Column<string>(type: "text", nullable: true),
                    JobKey = table.Column<string>(type: "text", nullable: false),
                    TriggerKey = table.Column<string>(type: "text", nullable: false),
                    ConsumableType = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "MealRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateEaten = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ServingsEaten = table.Column<float>(type: "real", nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PendingRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsumableType = table.Column<int>(type: "integer", nullable: false),
                    ConsumableId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DatePending = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealIngredientSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodMeasureId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "FoodPlans");

            migrationBuilder.DropTable(
                name: "FoodRecords");

            migrationBuilder.DropTable(
                name: "MealIngredientSnapshots");

            migrationBuilder.DropTable(
                name: "PendingRecords");

            migrationBuilder.DropTable(
                name: "MealRecords");
        }
    }
}
