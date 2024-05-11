using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleStore.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsOnTop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameSuggest",
                table: "ProductAttributes");

            migrationBuilder.CreateTable(
                name: "NewsOnTops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Header = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsOnTops", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsOnTops");

            migrationBuilder.AddColumn<string>(
                name: "NameSuggest",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
