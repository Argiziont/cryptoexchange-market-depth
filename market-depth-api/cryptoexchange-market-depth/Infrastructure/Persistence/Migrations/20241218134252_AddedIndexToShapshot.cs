using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoexchangeMarketDepth.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexToShapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_AcquiredAt",
                table: "Snapshots",
                column: "AcquiredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Snapshots_AcquiredAt",
                table: "Snapshots");
        }
    }
}
