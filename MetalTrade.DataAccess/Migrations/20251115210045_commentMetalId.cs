using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetalTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class commentMetalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MetalTypes_MetalTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MetalTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MetalTypeId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MetalTypeId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_MetalTypeId",
                table: "Products",
                column: "MetalTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MetalTypes_MetalTypeId",
                table: "Products",
                column: "MetalTypeId",
                principalTable: "MetalTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
