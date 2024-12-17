using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoexchangeMarketDepth.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcquiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Microtimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Asks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderBookSnapshotId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asks_Snapshots_OrderBookSnapshotId",
                        column: x => x.OrderBookSnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderBookSnapshotId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bids_Snapshots_OrderBookSnapshotId",
                        column: x => x.OrderBookSnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asks_OrderBookSnapshotId",
                table: "Asks",
                column: "OrderBookSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_OrderBookSnapshotId",
                table: "Bids",
                column: "OrderBookSnapshotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asks");

            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Snapshots");
        }
    }
}
