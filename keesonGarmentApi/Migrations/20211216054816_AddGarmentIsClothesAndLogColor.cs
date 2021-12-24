using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keesonGarmentApi.Migrations
{
    public partial class AddGarmentIsClothesAndLogColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "GarmentsAssignedLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "GarmentsAssignedLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClothes",
                table: "Garments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "GarmentsAssignedLogs");

            migrationBuilder.DropColumn(
                name: "IsClothes",
                table: "Garments");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "GarmentsAssignedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
