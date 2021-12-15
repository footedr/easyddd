using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDdd.ShipmentManagement.Data.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ShipmentManagement");

            migrationBuilder.CreateSequence(
                name: "DispatchNumbers",
                schema: "ShipmentManagement",
                startValue: 1000L);

            migrationBuilder.CreateSequence(
                name: "ShipmentIds",
                schema: "ShipmentManagement",
                startValue: 1000L);

            migrationBuilder.CreateTable(
                name: "Shipments",
                schema: "ShipmentManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReadyWindow_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadyWindow_Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReadyWindow_End = table.Column<TimeSpan>(type: "time", nullable: false),
                    Shipper_Address_Line1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_Address_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_Address_StateAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_Address_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_Address_Line2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipper_Contact_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_Contact_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipper_Contact_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Consignee_Address_Line1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_Address_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_Address_StateAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_Address_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_Address_Line2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Consignee_Contact_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_Contact_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Consignee_Contact_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    CarrierRate_Carrier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CarrierRate_FuelCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CarrierRate_DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CarrierRate_ChargeTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CarrierRate_Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DispatchInfo_DispatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_PickupNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_DispatchDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchInfo_CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    DispatchInfo_PickupNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "ShipmentDetails",
                schema: "ShipmentManagement",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    HandlingUnitCount = table.Column<int>(type: "int", nullable: false),
                    PackagingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHazardous = table.Column<bool>(type: "bit", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDetails", x => new { x.ShipmentId, x.Id });
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "ShipmentManagement",
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackingHistory",
                schema: "ShipmentManagement",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Occurred = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingHistory", x => new { x.ShipmentId, x.Id });
                    table.ForeignKey(
                        name: "FK_TrackingHistory_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "ShipmentManagement",
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_Identifier",
                schema: "ShipmentManagement",
                table: "ShipmentDetails",
                column: "Identifier");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Identifier",
                schema: "ShipmentManagement",
                table: "Shipments",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Status",
                schema: "ShipmentManagement",
                table: "Shipments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingHistory_Type",
                schema: "ShipmentManagement",
                table: "TrackingHistory",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateItemCharges",
                schema: "ShipmentManagement");

            migrationBuilder.DropTable(
                name: "ShipmentDetails",
                schema: "ShipmentManagement");

            migrationBuilder.DropTable(
                name: "TrackingHistory",
                schema: "ShipmentManagement");

            migrationBuilder.DropTable(
                name: "Shipments",
                schema: "ShipmentManagement");

            migrationBuilder.DropSequence(
                name: "DispatchNumbers",
                schema: "ShipmentManagement");

            migrationBuilder.DropSequence(
                name: "ShipmentIds",
                schema: "ShipmentManagement");
        }
    }
}
