using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Auth.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f", 0, "22afe541-5e3d-46e1-853b-8a86b21352cd", "coolmailedu@gmail.com", false, false, null, "COOLMAILEDU@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEEhgfoW46Eua490DQAiJS7JmmNfLmSoHrw2UeKWttoc3vYWKOgulUBXV5uSJ+XUUGg==", null, false, "762e211e-1eea-42aa-a5e4-f1f349923ded", false, "Admin" });

            migrationBuilder.InsertData(
                table: "NutrientTrackings",
                columns: new[] { "Id", "IsActive", "NutrientId", "TargetAmount", "UserId" },
                values: new object[,]
                {
                    { 162L, false, 301L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 163L, false, 205L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 164L, false, 601L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 165L, false, 208L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 166L, false, 606L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 167L, false, 204L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 168L, false, 605L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 169L, false, 303L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 170L, false, 291L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 171L, false, 306L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 172L, false, 307L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 173L, false, 203L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 174L, false, 269L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 175L, false, 539L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 176L, false, 324L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 177L, false, 299L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 178L, false, 1001L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 179L, false, 1006L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 180L, false, 1002L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 181L, false, 290L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 182L, false, 261L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 183L, false, 260L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 184L, false, 1003L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 185L, false, 1004L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 186L, false, 1005L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 187L, false, 513L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 188L, false, 221L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 189L, false, 511L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 190L, false, 207L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 191L, false, 514L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 192L, false, 454L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 193L, false, 262L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 194L, false, 639L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 195L, false, 322L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 196L, false, 321L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 197L, false, 326L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 198L, false, 421L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 199L, false, 334L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 200L, false, 312L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 201L, false, 507L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 202L, false, 268L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 203L, false, 325L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 204L, false, 610L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 205L, false, 611L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 206L, false, 696L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 207L, false, 612L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 208L, false, 625L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 209L, false, 652L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 210L, false, 697L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 211L, false, 613L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 212L, false, 626L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 213L, false, 673L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 214L, false, 662L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 215L, false, 653L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 216L, false, 687L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 217L, false, 614L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 218L, false, 617L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 219L, false, 674L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 220L, false, 663L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 221L, false, 859L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 222L, false, 618L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 223L, false, 670L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 224L, false, 675L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 225L, false, 669L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 226L, false, 619L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 227L, false, 851L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 228L, false, 685L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 229L, false, 627L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 230L, false, 615L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 231L, false, 628L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 232L, false, 672L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 233L, false, 689L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 234L, false, 852L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 235L, false, 853L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 236L, false, 620L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 237L, false, 855L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 238L, false, 629L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 239L, false, 857L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 240L, false, 624L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 241L, false, 630L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 242L, false, 858L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 243L, false, 631L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 244L, false, 621L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 245L, false, 654L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 246L, false, 671L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 247L, false, 607L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 248L, false, 608L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 249L, false, 609L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 250L, false, 645L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 251L, false, 646L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 252L, false, 693L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 253L, false, 695L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 254L, false, 313L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 255L, false, 417L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 256L, false, 431L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 257L, false, 435L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 258L, false, 432L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 259L, false, 212L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 260L, false, 287L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 261L, false, 515L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 262L, false, 211L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 263L, false, 516L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 264L, false, 512L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 265L, false, 521L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 266L, false, 503L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 267L, false, 213L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 268L, false, 504L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 269L, false, 338L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 270L, false, 337L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 271L, false, 505L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 272L, false, 214L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 273L, false, 506L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 274L, false, 304L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 275L, false, 428L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 276L, false, 315L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 277L, false, 406L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 278L, false, 573L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 279L, false, 578L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 280L, false, 257L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 281L, false, 664L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 282L, false, 676L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 283L, false, 856L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 284L, false, 665L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 285L, false, 666L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 286L, false, 305L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 287L, false, 410L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 288L, false, 508L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 289L, false, 636L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 290L, false, 517L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 291L, false, 319L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 292L, false, 405L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 293L, false, 317L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 294L, false, 518L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 295L, false, 641L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 296L, false, 209L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 297L, false, 638L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 298L, false, 210L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 299L, false, 263L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 300L, false, 404L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 301L, false, 502L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 302L, false, 323L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 303L, false, 341L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 304L, false, 343L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 305L, false, 342L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 306L, false, 501L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 307L, false, 509L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 308L, false, 510L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 309L, false, 318L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 310L, false, 320L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 311L, false, 418L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 312L, false, 415L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 313L, false, 401L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 314L, false, 328L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 315L, false, 430L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 316L, false, 429L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 317L, false, 255L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 318L, false, 309L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 319L, false, 344L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 320L, false, 345L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 321L, false, 346L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" },
                    { 322L, false, 347L, 0f, "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "9d75c886-0a61-40a1-8740-aaf027b8572f", "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "9d75c886-0a61-40a1-8740-aaf027b8572f", "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f" });

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 162L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 163L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 164L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 165L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 166L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 167L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 168L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 169L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 170L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 171L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 172L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 173L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 174L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 175L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 176L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 177L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 178L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 179L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 180L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 181L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 182L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 183L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 184L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 185L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 186L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 187L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 188L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 189L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 190L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 191L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 192L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 193L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 194L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 195L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 196L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 197L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 198L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 199L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 200L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 201L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 202L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 203L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 204L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 205L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 206L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 207L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 208L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 209L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 210L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 211L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 212L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 213L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 214L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 215L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 216L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 217L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 218L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 219L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 220L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 221L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 222L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 223L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 224L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 225L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 226L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 227L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 228L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 229L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 230L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 231L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 232L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 233L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 234L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 235L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 236L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 237L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 238L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 239L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 240L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 241L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 242L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 243L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 244L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 245L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 246L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 247L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 248L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 249L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 250L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 251L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 252L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 253L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 254L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 255L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 256L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 257L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 258L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 259L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 260L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 261L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 262L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 263L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 264L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 265L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 266L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 267L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 268L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 269L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 270L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 271L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 272L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 273L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 274L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 275L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 276L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 277L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 278L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 279L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 280L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 281L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 282L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 283L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 284L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 285L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 286L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 287L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 288L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 289L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 290L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 291L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 292L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 293L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 294L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 295L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 296L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 297L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 298L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 299L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 300L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 301L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 302L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 303L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 304L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 305L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 306L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 307L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 308L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 309L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 310L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 311L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 312L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 313L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 314L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 315L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 316L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 317L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 318L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 319L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 320L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 321L);

            migrationBuilder.DeleteData(
                table: "NutrientTrackings",
                keyColumn: "Id",
                keyValue: 322L);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f");
        }
    }
}
