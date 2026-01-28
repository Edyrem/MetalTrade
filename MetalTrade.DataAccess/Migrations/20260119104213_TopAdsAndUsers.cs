using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MetalTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TopAdsAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commercials_Advertisements_AdvertisementId",
                table: "Commercials");

            migrationBuilder.RenameColumn(
                name: "AdStartDate",
                table: "Commercials",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "AdEndDate",
                table: "Commercials",
                newName: "EndDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "Commercials",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Commercials",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Commercials",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Commercials",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTop",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TopAdvertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdvertisementId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopAdvertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopAdvertisements_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopAdvertisements_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TargetUserId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopUsers_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopUsers_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commercials_CreatedByUserId",
                table: "Commercials",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TopAdvertisements_AdvertisementId",
                table: "TopAdvertisements",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_TopAdvertisements_CreatedByUserId",
                table: "TopAdvertisements",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TopUsers_CreatedByUserId",
                table: "TopUsers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TopUsers_TargetUserId",
                table: "TopUsers",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commercials_Advertisements_AdvertisementId",
                table: "Commercials",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Commercials_AspNetUsers_CreatedByUserId",
                table: "Commercials",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commercials_Advertisements_AdvertisementId",
                table: "Commercials");

            migrationBuilder.DropForeignKey(
                name: "FK_Commercials_AspNetUsers_CreatedByUserId",
                table: "Commercials");

            migrationBuilder.DropTable(
                name: "TopAdvertisements");

            migrationBuilder.DropTable(
                name: "TopUsers");

            migrationBuilder.DropIndex(
                name: "IX_Commercials_CreatedByUserId",
                table: "Commercials");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Commercials");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Commercials");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Commercials");

            migrationBuilder.DropColumn(
                name: "IsTop",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Commercials",
                newName: "AdStartDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Commercials",
                newName: "AdEndDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "Commercials",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Commercials_Advertisements_AdvertisementId",
                table: "Commercials",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
