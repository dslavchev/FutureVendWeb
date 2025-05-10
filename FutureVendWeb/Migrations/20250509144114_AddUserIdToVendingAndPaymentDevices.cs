using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutureVendWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToVendingAndPaymentDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "VendingDevices",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PaymentDevices",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Customers",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.CreateIndex(
                name: "IX_VendingDevices_UserId",
                table: "VendingDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDevices_UserId",
                table: "PaymentDevices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDevices_AspNetUsers_UserId",
                table: "PaymentDevices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendingDevices_AspNetUsers_UserId",
                table: "VendingDevices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDevices_AspNetUsers_UserId",
                table: "PaymentDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_VendingDevices_AspNetUsers_UserId",
                table: "VendingDevices");

            migrationBuilder.DropIndex(
                name: "IX_VendingDevices_UserId",
                table: "VendingDevices");

            migrationBuilder.DropIndex(
                name: "IX_PaymentDevices_UserId",
                table: "PaymentDevices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VendingDevices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PaymentDevices");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Customers",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
