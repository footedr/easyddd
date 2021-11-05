using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyDdd.Data.Migrations
{
    public partial class ShipmentRateInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarrierRate_Carrier",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarrierRate_ChargeTotal",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarrierRate_DiscountAmount",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarrierRate_FuelCharge",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarrierRate_Total",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RateItemCharges",
                schema: "ShipmentManagement",
                columns: table => new
                {
                    RateShipmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateItemCharges", x => new { x.RateShipmentId, x.Id });
                    table.ForeignKey(
                        name: "FK_RateItemCharges_Shipments_RateShipmentId",
                        column: x => x.RateShipmentId,
                        principalSchema: "ShipmentManagement",
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateItemCharges",
                schema: "ShipmentManagement");

            migrationBuilder.DropColumn(
                name: "CarrierRate_Carrier",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CarrierRate_ChargeTotal",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CarrierRate_DiscountAmount",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CarrierRate_FuelCharge",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CarrierRate_Total",
                schema: "ShipmentManagement",
                table: "Shipments");
        }
    }
}
