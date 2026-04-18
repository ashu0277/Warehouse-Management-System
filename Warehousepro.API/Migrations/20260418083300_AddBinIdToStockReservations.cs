using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehousepro.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBinIdToStockReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BinID",
                table: "StockReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_BinID",
                table: "StockReservations",
                column: "BinID");

            migrationBuilder.AddForeignKey(
                name: "FK_StockReservations_BinLocations_BinID",
                table: "StockReservations",
                column: "BinID",
                principalTable: "BinLocations",
                principalColumn: "BinID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockReservations_BinLocations_BinID",
                table: "StockReservations");

            migrationBuilder.DropIndex(
                name: "IX_StockReservations_BinID",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "BinID",
                table: "StockReservations");
        }
    }
}
