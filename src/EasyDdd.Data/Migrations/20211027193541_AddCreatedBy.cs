using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyDdd.Data.Migrations
{
    public partial class AddCreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "ShipmentManagement",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "ShipmentManagement",
                table: "Shipments");
        }
    }
}
