using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyDdd.Data.Migrations
{
    public partial class DispatchShipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "DispatchNumbers",
                schema: "ShipmentManagement",
                startValue: 1000L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DispatchInfo_Created",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "datetimeoffset",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "DispatchInfo_CreatedBy",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchInfo_DispatchDateTime",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispatchInfo_DispatchNumber",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispatchInfo_PickupNote",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispatchInfo_PickupNumber",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispatchInfo_ReferenceNumber",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "DispatchNumbers",
                schema: "ShipmentManagement");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_Created",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_CreatedBy",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_DispatchDateTime",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_DispatchNumber",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_PickupNote",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_PickupNumber",
                schema: "ShipmentManagement",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchInfo_ReferenceNumber",
                schema: "ShipmentManagement",
                table: "Shipments");
        }
    }
}
