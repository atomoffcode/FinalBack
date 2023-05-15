using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazerFinal.Migrations
{
    public partial class UpdatedOrdersTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_SingleAddressId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "SingleAddressId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "CustomAddress",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_SingleAddressId",
                table: "Orders",
                column: "SingleAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_SingleAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomAddress",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "SingleAddressId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_SingleAddressId",
                table: "Orders",
                column: "SingleAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
