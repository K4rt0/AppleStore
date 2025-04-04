﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleStore.Migrations
{
    /// <inheritdoc />
    public partial class Update_AddToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_AspNetUsers_ApplicationUserId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ApplicationUserId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CartItems");
        }
    }
}
