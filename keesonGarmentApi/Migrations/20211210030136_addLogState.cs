using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keesonGarmentApi.Migrations
{
    public partial class addLogState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssiged",
                table: "GarmentsAssignedLogs");

            migrationBuilder.DropColumn(
                name: "IsRefund",
                table: "GarmentsAssignedLogs");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "GarmentsAssignedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "GarmentsAssignedLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "GarmentsAssignedLogs");

            migrationBuilder.DropColumn(
                name: "State",
                table: "GarmentsAssignedLogs");

            migrationBuilder.AddColumn<bool>(
                name: "IsAssiged",
                table: "GarmentsAssignedLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefund",
                table: "GarmentsAssignedLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
