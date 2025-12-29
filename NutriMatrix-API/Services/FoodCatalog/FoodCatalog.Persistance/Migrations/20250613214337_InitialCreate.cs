using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodCatalog.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Barcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TotalServings = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nutrients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutrients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Measures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    WeightInGrams = table.Column<float>(type: "real", nullable: false),
                    FoodId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Measures_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodNutrientIn100Gs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodId = table.Column<long>(type: "bigint", nullable: false),
                    NutrientId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutrientIn100Gs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodNutrientIn100Gs_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodNutrientIn100Gs_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodMeals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MeasureId = table.Column<long>(type: "bigint", nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodMeals_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodMeals_Measures_MeasureId",
                        column: x => x.MeasureId,
                        principalTable: "Measures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "IsDeleted", "Name", "Unit" },
                values: new object[,]
                {
                    { 203L, false, "Protein", "g" },
                    { 204L, false, "Total fat", "g" },
                    { 205L, false, "Carbohydrate, by difference", "g" },
                    { 207L, false, "Ash", "g" },
                    { 208L, false, "Energy", "kcal" },
                    { 209L, false, "Starch", "g" },
                    { 210L, false, "Sucrose", "g" },
                    { 211L, false, "Glucose (dextrose)", "g" },
                    { 212L, false, "Fructose", "g" },
                    { 213L, false, "Lactose", "g" },
                    { 214L, false, "Maltose", "g" },
                    { 221L, false, "Alcohol, ethyl", "g" },
                    { 255L, false, "Water", "g" },
                    { 257L, false, "Adjusted Protein", "g" },
                    { 260L, false, "Mannitol", "g" },
                    { 261L, false, "Sorbitol", "g" },
                    { 262L, false, "Caffeine", "mg" },
                    { 263L, false, "Theobromine", "mg" },
                    { 268L, false, "Energy", "kJ" },
                    { 269L, false, "Sugars, total", "g" },
                    { 287L, false, "Galactose", "g" },
                    { 290L, false, "Xylitol", "g" },
                    { 291L, false, "Fiber, total dietary", "g" },
                    { 299L, false, "Sugar Alcohol", "g" },
                    { 301L, false, "Calcium, Ca", "mg" },
                    { 303L, false, "Iron, Fe", "mg" },
                    { 304L, false, "Magnesium, Mg", "mg" },
                    { 305L, false, "Phosphorus, P", "mg" },
                    { 306L, false, "Potassium, K", "mg" },
                    { 307L, false, "Sodium, Na", "mg" },
                    { 309L, false, "Zinc, Zn", "mg" },
                    { 312L, false, "Copper, Cu", "mg" },
                    { 313L, false, "Fluoride, F", "µg" },
                    { 315L, false, "Manganese, Mn", "mg" },
                    { 317L, false, "Selenium, Se", "µg" },
                    { 318L, false, "Vitamin A, IU", "IU" },
                    { 319L, false, "Retinol", "µg" },
                    { 320L, false, "Vitamin A, RAE", "µg" },
                    { 321L, false, "Carotene, beta", "µg" },
                    { 322L, false, "Carotene, alpha", "µg" },
                    { 323L, false, "Vitamin E (alpha-tocopherol)", "mg" },
                    { 324L, false, "Vitamin D", "IU" },
                    { 325L, false, "Vitamin D2 (ergocalciferol)", "µg" },
                    { 326L, false, "Vitamin D3 (cholecalciferol)", "µg" },
                    { 328L, false, "Vitamin D (D2 + D3)", "µg" },
                    { 334L, false, "Cryptoxanthin, beta", "µg" },
                    { 337L, false, "Lycopene", "µg" },
                    { 338L, false, "Lutein + zeaxanthin", "µg" },
                    { 341L, false, "Tocopherol, beta", "mg" },
                    { 342L, false, "Tocopherol, gamma", "mg" },
                    { 343L, false, "Tocopherol, delta", "mg" },
                    { 344L, false, "Tocotrienol, alpha", "mg" },
                    { 345L, false, "Tocotrienol, beta", "mg" },
                    { 346L, false, "Tocotrienol, gamma", "mg" },
                    { 347L, false, "Tocotrienol,delta", "mg" },
                    { 401L, false, "Vitamin C, total ascorbic acid", "mg" },
                    { 404L, false, "Thiamin", "mg" },
                    { 405L, false, "Riboflavin", "mg" },
                    { 406L, false, "Niacin", "mg" },
                    { 410L, false, "Pantothenic acid", "mg" },
                    { 415L, false, "Vitamin B-6", "mg" },
                    { 417L, false, "Folate, total", "µg" },
                    { 418L, false, "Vitamin B-12", "µg" },
                    { 421L, false, "Choline, total", "mg" },
                    { 428L, false, "Menaquinone-4", "µg" },
                    { 429L, false, "Dihydrophylloquinone", "µg" },
                    { 430L, false, "Vitamin K (phylloquinone)", "µg" },
                    { 431L, false, "Folic acid", "µg" },
                    { 432L, false, "Folate, food", "µg" },
                    { 435L, false, "Folate, DFE", "µg" },
                    { 454L, false, "Betaine", "mg" },
                    { 501L, false, "Tryptophan", "g" },
                    { 502L, false, "Threonine", "g" },
                    { 503L, false, "Isoleucine", "g" },
                    { 504L, false, "Leucine", "g" },
                    { 505L, false, "Lysine", "g" },
                    { 506L, false, "Methionine", "g" },
                    { 507L, false, "Cystine", "g" },
                    { 508L, false, "Phenylalanine", "g" },
                    { 509L, false, "Tyrosine", "g" },
                    { 510L, false, "Valine", "g" },
                    { 511L, false, "Arginine", "g" },
                    { 512L, false, "Histidine", "g" },
                    { 513L, false, "Alanine", "g" },
                    { 514L, false, "Aspartic acid", "g" },
                    { 515L, false, "Glutamic acid", "g" },
                    { 516L, false, "Glycine", "g" },
                    { 517L, false, "Proline", "g" },
                    { 518L, false, "Serine", "g" },
                    { 521L, false, "Hydroxyproline", "g" },
                    { 539L, false, "Sugars, added", "g" },
                    { 573L, false, "Vitamin E, added", "mg" },
                    { 578L, false, "Vitamin B-12, added", "µg" },
                    { 601L, false, "Cholesterol", "mg" },
                    { 605L, false, "Fatty acids, total trans", "g" },
                    { 606L, false, "Fatty acids, total saturated", "g" },
                    { 607L, false, "4:00", "g" },
                    { 608L, false, "6:00", "g" },
                    { 609L, false, "8:00", "g" },
                    { 610L, false, "10:00", "g" },
                    { 611L, false, "12:00", "g" },
                    { 612L, false, "14:00", "g" },
                    { 613L, false, "16:00", "g" },
                    { 614L, false, "18:00", "g" },
                    { 615L, false, "20:00", "g" },
                    { 617L, false, "18:1 undifferentiated", "g" },
                    { 618L, false, "18:2 undifferentiated", "g" },
                    { 619L, false, "18:3 undifferentiated", "g" },
                    { 620L, false, "20:4 undifferentiated", "g" },
                    { 621L, false, "22:6 n-3 (DHA)", "g" },
                    { 624L, false, "22:00", "g" },
                    { 625L, false, "14:01", "g" },
                    { 626L, false, "16:1 undifferentiated", "g" },
                    { 627L, false, "18:04", "g" },
                    { 628L, false, "20:01", "g" },
                    { 629L, false, "20:5 n-3 (EPA)", "g" },
                    { 630L, false, "22:1 undifferentiated", "g" },
                    { 631L, false, "22:5 n-3 (DPA)", "g" },
                    { 636L, false, "Phytosterols", "mg" },
                    { 638L, false, "Stigmasterol", "mg" },
                    { 639L, false, "Campesterol", "mg" },
                    { 641L, false, "Beta-sitosterol", "mg" },
                    { 645L, false, "Fatty acids, total monounsaturated", "g" },
                    { 646L, false, "Fatty acids, total polyunsaturated", "g" },
                    { 652L, false, "15:00", "g" },
                    { 653L, false, "17:00", "g" },
                    { 654L, false, "24:00:00", "g" },
                    { 662L, false, "16:1 t", "g" },
                    { 663L, false, "18:1 t", "g" },
                    { 664L, false, "22:1 t", "g" },
                    { 665L, false, "18:2 t not further defined", "g" },
                    { 666L, false, "18:2 i", "g" },
                    { 669L, false, "18:2 t,t", "g" },
                    { 670L, false, "18:2 CLAs", "g" },
                    { 671L, false, "24:1 c", "g" },
                    { 672L, false, "20:2 n-6 c,c", "g" },
                    { 673L, false, "16:1 c", "g" },
                    { 674L, false, "18:1 c", "g" },
                    { 675L, false, "18:2 n-6 c,c", "g" },
                    { 676L, false, "22:1 c", "g" },
                    { 685L, false, "18:3 n-6 c,c,c", "g" },
                    { 687L, false, "17:01", "g" },
                    { 689L, false, "20:3 undifferentiated", "g" },
                    { 693L, false, "Fatty acids, total trans-monoenoic", "g" },
                    { 695L, false, "Fatty acids, total trans-polyenoic", "g" },
                    { 696L, false, "13:00", "g" },
                    { 697L, false, "15:01", "g" },
                    { 851L, false, "18:3 n-3 c,c,c (ALA)", "g" },
                    { 852L, false, "20:3 n-3", "g" },
                    { 853L, false, "20:3 n-6", "g" },
                    { 855L, false, "20:4 n-6", "g" },
                    { 856L, false, "18:3i", "g" },
                    { 857L, false, "21:05", "g" },
                    { 858L, false, "22:04", "g" },
                    { 859L, false, "18:1-11t (18:1t n-7)", "g" },
                    { 1001L, false, "Erythritol", "g" },
                    { 1002L, false, "Glycerin", "g" },
                    { 1003L, false, "Maltitol", "g" },
                    { 1004L, false, "Isomalt", "g" },
                    { 1005L, false, "Lactitol", "g" },
                    { 1006L, false, "Allulose", "g" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodMeals_MealId",
                table: "FoodMeals",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodMeals_MeasureId",
                table: "FoodMeals",
                column: "MeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutrientIn100Gs_FoodId",
                table: "FoodNutrientIn100Gs",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutrientIn100Gs_NutrientId",
                table: "FoodNutrientIn100Gs",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_Measures_FoodId",
                table: "Measures",
                column: "FoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodMeals");

            migrationBuilder.DropTable(
                name: "FoodNutrientIn100Gs");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Measures");

            migrationBuilder.DropTable(
                name: "Nutrients");

            migrationBuilder.DropTable(
                name: "Foods");
        }
    }
}
