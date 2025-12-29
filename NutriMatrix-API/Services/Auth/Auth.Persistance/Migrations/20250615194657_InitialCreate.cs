using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Auth.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutrientTrackings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    NutrientId = table.Column<long>(type: "bigint", nullable: false),
                    TargetAmount = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientTrackings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "51d825fc-b536-4568-9679-7e8eac698ebe", "51d825fc-b536-4568-9679-7e8eac698ebe", "User", "USER" },
                    { "9d75c886-0a61-40a1-8740-aaf027b8572f", "9d75c886-0a61-40a1-8740-aaf027b8572f", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f", 0, "4bba7b4c-46be-4e5a-99c3-af68d35ef9b0", "coolmailedu@gmail.com", false, false, null, "COOLMAILEDU@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEJzHPFuc2hfuHryBcAqo1JOjnmRC4PIV9Z9MamzH+u5ZcxgOOdIj+m4L8BEmyXJa/g==", null, false, "85235144-51ac-480e-bb5f-6c068813bfcd", false, "Admin" });

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "NutrientTrackings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
