using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDdd.Billing.Data.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.CreateSequence(
                name: "StatementIdentifiers",
                schema: "billing",
                startValue: 10000L);

            migrationBuilder.CreateTable(
                name: "Shipments",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Shipper_Line1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_StateAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shipper_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_Line1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_StateAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Carrier_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carrier_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_DispatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_PickupNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_DispatchDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchInfo_PickupNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchInfo_ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statements",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValueSql: "1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BillingPeriod_Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingPeriod_End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillToAccount = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BillToLocation = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDetails",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    HandlingUnitCount = table.Column<int>(type: "int", nullable: false),
                    IsHazardous = table.Column<bool>(type: "bit", nullable: false),
                    ShipmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "billing",
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatementLines",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TmsNumber = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandlingUnits = table.Column<int>(type: "int", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Weight = table.Column<double>(type: "float", nullable: true),
                    StatementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementLines_Statements_StatementId",
                        column: x => x.StatementId,
                        principalSchema: "billing",
                        principalTable: "Statements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_ShipmentId",
                schema: "billing",
                table: "ShipmentDetails",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Identifier",
                schema: "billing",
                table: "Shipments",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Status",
                schema: "billing",
                table: "Shipments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StatementLines_StatementId",
                schema: "billing",
                table: "StatementLines",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_BillToAccount",
                schema: "billing",
                table: "Statements",
                column: "BillToAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_BillToLocation",
                schema: "billing",
                table: "Statements",
                column: "BillToLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_CustomerCode",
                schema: "billing",
                table: "Statements",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_Identifier",
                schema: "billing",
                table: "Statements",
                column: "Identifier");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_ProcessedAt",
                schema: "billing",
                table: "Statements",
                column: "ProcessedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentDetails",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "StatementLines",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Shipments",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Statements",
                schema: "billing");

            migrationBuilder.DropSequence(
                name: "StatementIdentifiers",
                schema: "billing");
        }
    }
}
