using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankLedgerApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        // Existing rows (and the default tenant created below) are backfilled onto this fixed
        // id so a fresh dev database and one with prior local data both end up consistent.
        private static readonly Guid DefaultTenantId = new("11111111-1111-1111-1111-111111111111");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_DocumentNumber",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Number",
                table: "Accounts");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: ["Id", "Name", "Slug", "IsActive", "CreatedAt"],
                values: new object[] { DefaultTenantId, "Default Tenant", "default", true, new DateTime(2026, 8, 2, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Transfers",
                type: "TEXT",
                nullable: false,
                defaultValue: DefaultTenantId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: DefaultTenantId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: DefaultTenantId);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TenantId_Number",
                table: "Accounts",
                columns: new[] { "TenantId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            // Composite uniqueness across the owned TaxDocument.Number column: EF Core's fluent
            // HasIndex can't mix an owner property (TenantId) with a dotted path into an owned
            // type in the same call, so it's created here directly.
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX IX_Customers_TenantId_DocumentNumber ON Customers (TenantId, DocumentNumber);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IX_Customers_TenantId_DocumentNumber;");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_TenantId_Number",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DocumentNumber",
                table: "Customers",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Number",
                table: "Accounts",
                column: "Number",
                unique: true);
        }
    }
}
