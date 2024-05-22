using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeliveryAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_AspNetUsers_ApplicationUserId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ApplicationUserId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "City",
                table: "DeliveryAddresses");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "DeliveryAddresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "DeliveryAddresses");

            migrationBuilder.DropColumn(
                name: "State",
                table: "DeliveryAddresses");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CartItems");

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "DeliveryAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "DeliveryAddresses");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "DeliveryAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "DeliveryAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostalCode",
                table: "DeliveryAddresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "DeliveryAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ApplicationUserId",
                table: "CartItems",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_AspNetUsers_ApplicationUserId",
                table: "CartItems",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
