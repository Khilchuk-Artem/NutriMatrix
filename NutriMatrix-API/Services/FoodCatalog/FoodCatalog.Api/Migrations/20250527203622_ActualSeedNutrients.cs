using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodCatalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class ActualSeedNutrients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 203L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 204L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 205L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 207L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 208L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 209L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 210L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 211L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 212L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 213L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 214L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 221L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 255L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 257L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 260L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 261L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 262L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 263L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 268L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 269L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 287L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 290L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 291L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 299L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 301L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 303L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 304L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 305L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 306L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 307L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 309L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 312L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 313L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 315L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 317L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 318L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 319L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 320L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 321L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 322L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 323L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 324L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 325L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 326L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 328L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 334L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 337L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 338L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 341L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 342L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 343L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 344L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 345L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 346L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 347L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 401L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 404L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 405L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 406L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 410L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 415L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 417L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 418L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 421L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 428L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 429L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 430L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 431L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 432L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 435L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 454L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 501L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 502L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 503L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 504L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 505L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 506L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 507L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 508L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 509L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 510L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 511L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 512L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 513L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 514L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 515L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 516L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 517L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 518L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 521L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 539L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 573L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 578L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 601L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 605L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 606L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 607L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 608L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 609L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 610L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 611L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 612L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 613L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 614L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 615L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 617L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 618L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 619L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 620L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 621L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 624L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 625L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 626L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 627L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 628L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 629L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 630L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 631L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 636L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 638L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 639L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 641L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 645L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 646L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 652L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 653L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 654L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 662L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 663L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 664L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 665L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 666L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 669L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 670L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 671L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 672L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 673L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 674L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 675L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 676L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 685L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 687L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 689L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 693L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 695L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 696L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 697L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 851L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 852L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 853L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 855L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 856L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 857L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 858L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 859L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 1001L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 1002L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 1003L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 1004L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 1005L);

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: 1006L);
        }
    }
}
