using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDdd.Billing.Data.Migrations
{
    public partial class ShipmentLatestTrackingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LatesTrackingEvent_Comments",
                schema: "billing",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LatesTrackingEvent_Occurred",
                schema: "billing",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LatesTrackingEvent_Type",
                schema: "billing",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatesTrackingEvent_Comments",
                schema: "billing",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "LatesTrackingEvent_Occurred",
                schema: "billing",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "LatesTrackingEvent_Type",
                schema: "billing",
                table: "Shipments");
        }
    }
}
