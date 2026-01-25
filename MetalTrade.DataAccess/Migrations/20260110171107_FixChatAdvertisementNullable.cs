using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetalTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixChatAdvertisementNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Advertisements_AdvertisementId",
                table: "Chats");

            migrationBuilder.AlterColumn<int>(
                name: "AdvertisementId",
                table: "Chats",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Advertisements_AdvertisementId",
                table: "Chats",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Advertisements_AdvertisementId",
                table: "Chats");

            migrationBuilder.AlterColumn<int>(
                name: "AdvertisementId",
                table: "Chats",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Advertisements_AdvertisementId",
                table: "Chats",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
