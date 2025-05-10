using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutureVendWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToVendingProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "VendingProducts",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendingProducts_UserId",
                table: "VendingProducts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendingProducts_AspNetUsers_UserId",
                table: "VendingProducts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendingProducts_AspNetUsers_UserId",
                table: "VendingProducts");

            migrationBuilder.DropIndex(
                name: "IX_VendingProducts_UserId",
                table: "VendingProducts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VendingProducts");
        }
    }
}
