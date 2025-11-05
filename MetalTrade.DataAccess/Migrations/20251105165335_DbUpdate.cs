using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MetalTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DbUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_Cities_CityId",
                table: "Advertisements");

            migrationBuilder.DropTable(
                name: "AdvertisementProductProperties");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ProductProperties");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_CityId",
                table: "Advertisements");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Advertisements",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Advertisements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAd",
                table: "Advertisements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTop",
                table: "Advertisements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Advertisements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "IsAd",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "IsTop",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Advertisements");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Advertisements",
                newName: "CityId");

            migrationBuilder.CreateTable(
                name: "ProductProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementProductProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdvertisementId = table.Column<int>(type: "integer", nullable: false),
                    PropertyId = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementProductProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementProductProperties_Advertisements_Advertisement~",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvertisementProductProperties_ProductProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "ProductProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_CityId",
                table: "Advertisements",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementProductProperties_AdvertisementId",
                table: "AdvertisementProductProperties",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementProductProperties_PropertyId",
                table: "AdvertisementProductProperties",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_RegionId",
                table: "Cities",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_Cities_CityId",
                table: "Advertisements",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
