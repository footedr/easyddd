using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyDdd.Data.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ShipmentManagement");

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
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentDetails",
                schema: "ShipmentManagement");

            migrationBuilder.DropTable(
                name: "Shipments",
                schema: "ShipmentManagement");
        }
    }
}
