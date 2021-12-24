using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keesonGarmentApi.Migrations
{
    public partial class addColGEmployeeIsdelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GEmployees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GEmployees");
        }
    }
}
