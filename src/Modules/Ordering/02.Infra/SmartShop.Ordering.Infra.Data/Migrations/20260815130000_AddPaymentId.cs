using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartShop.Ordering.Infra.Data.Migrations
{
    [DbContext(typeof(OrderingDbContext))]
    [Migration("20260815130000_AddPaymentId")]
    public partial class AddPaymentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                schema: "ordering",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentId",
                schema: "ordering",
                table: "Orders",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentId",
                schema: "ordering",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                schema: "ordering",
                table: "Orders");
        }
    }
}
