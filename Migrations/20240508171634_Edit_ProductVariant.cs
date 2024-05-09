using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleStore.Migrations
{
    /// <inheritdoc />
    public partial class Edit_ProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_BatteryId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_CameraId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ColorId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ConnectivityId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_DisplaySizeId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_MemoryId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_OperatingId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ProcessorId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ResolutionId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_StorageCapacityId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_BatteryId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_CameraId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ColorId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ConnectivityId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_DisplaySizeId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_MemoryId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_OperatingId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProcessorId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ResolutionId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_StorageCapacityId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "BatteryId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "CameraId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ConnectivityId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "DisplaySizeId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "MemoryId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "OperatingId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ProcessorId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ResolutionId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "StorageCapacityId",
                table: "ProductVariants");

            migrationBuilder.CreateTable(
                name: "VariantsAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeValueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantsAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantsAttributes_ProductAttributeValues_ProductAttributeValueId",
                        column: x => x.ProductAttributeValueId,
                        principalTable: "ProductAttributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariantsAttributes_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariantsAttributes_ProductAttributeValueId",
                table: "VariantsAttributes",
                column: "ProductAttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantsAttributes_ProductVariantId",
                table: "VariantsAttributes",
                column: "ProductVariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VariantsAttributes");

            migrationBuilder.AddColumn<int>(
                name: "BatteryId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CameraId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConnectivityId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplaySizeId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemoryId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OperatingId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessorId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResolutionId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageCapacityId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_BatteryId",
                table: "ProductVariants",
                column: "BatteryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_CameraId",
                table: "ProductVariants",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ColorId",
                table: "ProductVariants",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ConnectivityId",
                table: "ProductVariants",
                column: "ConnectivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_DisplaySizeId",
                table: "ProductVariants",
                column: "DisplaySizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_MemoryId",
                table: "ProductVariants",
                column: "MemoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_OperatingId",
                table: "ProductVariants",
                column: "OperatingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProcessorId",
                table: "ProductVariants",
                column: "ProcessorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ResolutionId",
                table: "ProductVariants",
                column: "ResolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_StorageCapacityId",
                table: "ProductVariants",
                column: "StorageCapacityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_BatteryId",
                table: "ProductVariants",
                column: "BatteryId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_CameraId",
                table: "ProductVariants",
                column: "CameraId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ColorId",
                table: "ProductVariants",
                column: "ColorId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ConnectivityId",
                table: "ProductVariants",
                column: "ConnectivityId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_DisplaySizeId",
                table: "ProductVariants",
                column: "DisplaySizeId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_MemoryId",
                table: "ProductVariants",
                column: "MemoryId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_OperatingId",
                table: "ProductVariants",
                column: "OperatingId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ProcessorId",
                table: "ProductVariants",
                column: "ProcessorId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_ResolutionId",
                table: "ProductVariants",
                column: "ResolutionId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductAttributeValues_StorageCapacityId",
                table: "ProductVariants",
                column: "StorageCapacityId",
                principalTable: "ProductAttributeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
