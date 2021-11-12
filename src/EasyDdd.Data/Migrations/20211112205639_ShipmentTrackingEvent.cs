using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyDdd.Data.Migrations
{
    public partial class ShipmentTrackingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_TrackingHistory_Type",
                schema: "ShipmentManagement",
                table: "TrackingHistory",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingHistory",
                schema: "ShipmentManagement");
        }
    }
}
