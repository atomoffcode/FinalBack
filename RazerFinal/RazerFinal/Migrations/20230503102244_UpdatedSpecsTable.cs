using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazerFinal.Migrations
{
    public partial class UpdatedSpecsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Specifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Specifications");
        }
    }
}
