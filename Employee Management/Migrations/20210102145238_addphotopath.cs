using Microsoft.EntityFrameworkCore.Migrations;

namespace Employee_Management.Migrations
{
    public partial class addphotopath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phtotpath",
                table: "Employees",
                newName: "Photopath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photopath",
                table: "Employees",
                newName: "Phtotpath");
        }
    }
}
