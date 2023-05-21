using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazerFinal.Migrations
{
    public partial class UpdatedIdexPostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndexPosts_Products_ProductId",
                table: "IndexPosts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "IndexPosts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_IndexPosts_Products_ProductId",
                table: "IndexPosts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndexPosts_Products_ProductId",
                table: "IndexPosts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "IndexPosts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IndexPosts_Products_ProductId",
                table: "IndexPosts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
