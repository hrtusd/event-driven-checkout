using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventDrivenCheckout.Order.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShippedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippedAt",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }
    }
}
