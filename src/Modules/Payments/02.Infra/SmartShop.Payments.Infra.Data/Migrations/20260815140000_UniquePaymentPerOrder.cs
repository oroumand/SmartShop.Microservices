using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartShop.Payments.Infra.Data.Migrations
{
    public partial class UniquePaymentPerOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                schema: "payments",
                table: "Payments",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                schema: "payments",
                table: "Payments");
        }
    }
}
