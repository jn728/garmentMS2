using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keesonGarmentApi.Migrations
{
    public partial class updateSizeAndLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClothesSize",
                table: "GarmentsAssignedLogs");

            migrationBuilder.DropColumn(
                name: "ShoesSize",
                table: "GarmentsAssignedLogs");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "GarmentsSizes",
                newName: "ShoesSize");

            migrationBuilder.AddColumn<string>(
                name: "ClothesSize",
                table: "GarmentsSizes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClothesSize",
                table: "GarmentsSizes");

            migrationBuilder.RenameColumn(
                name: "ShoesSize",
                table: "GarmentsSizes",
                newName: "Size");

            migrationBuilder.AddColumn<string>(
                name: "ClothesSize",
                table: "GarmentsAssignedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShoesSize",
                table: "GarmentsAssignedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
