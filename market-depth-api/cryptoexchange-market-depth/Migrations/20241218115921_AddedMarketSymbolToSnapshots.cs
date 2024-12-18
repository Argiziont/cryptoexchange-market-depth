using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoexchangeMarketDepth.Migrations
{
    /// <inheritdoc />
    public partial class AddedMarketSymbolToSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketSymbol",
                table: "Snapshots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketSymbol",
                table: "Snapshots");
        }
    }
}
