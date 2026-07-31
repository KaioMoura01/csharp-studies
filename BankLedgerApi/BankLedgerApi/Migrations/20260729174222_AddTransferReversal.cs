using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankLedgerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferReversal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReversedTransferId",
                table: "Transfers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ReversedTransferId",
                table: "Transfers",
                column: "ReversedTransferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_ReversedTransferId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "ReversedTransferId",
                table: "Transfers");
        }
    }
}
