using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleStore.Migrations
{
    /// <inheritdoc />
    public partial class AddNameSuggest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameSuggest",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameSuggest",
                table: "ProductAttributes");
        }
    }
}
