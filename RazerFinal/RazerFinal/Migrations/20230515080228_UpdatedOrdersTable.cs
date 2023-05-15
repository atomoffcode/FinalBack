using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazerFinal.Migrations
{
    public partial class UpdatedOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SingleAddressId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SingleAddressId",
                table: "Orders",
                column: "SingleAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_SingleAddressId",
                table: "Orders",
                column: "SingleAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_SingleAddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SingleAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SingleAddressId",
                table: "Orders");
        }
    }
}
