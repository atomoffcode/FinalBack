using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazerFinal.Migrations
{
    public partial class UpdatedAddressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Addresses");
        }
    }
}
