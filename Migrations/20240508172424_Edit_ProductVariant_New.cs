using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleStore.Migrations
{
    /// <inheritdoc />
    public partial class Edit_ProductVariant_New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ProductAttributeValueId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductAttributeValueId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ProductAttributeValueId",
                table: "ProductVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductAttributeValueId",
                table: "ProductVariants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductAttributeValueId",
                table: "ProductVariants",
                column: "ProductAttributeValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ProductAttributeValueId",
                table: "ProductVariants",
                column: "ProductAttributeValueId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id");
        }
    }
}
