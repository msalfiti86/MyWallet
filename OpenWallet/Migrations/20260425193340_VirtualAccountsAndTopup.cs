using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWallet.Migrations
{
    /// <inheritdoc />
    public partial class VirtualAccountsAndTopup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "WalletTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VirtualIban",
                table: "WalletTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VirtualIban",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstNameAr",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstNameEn",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdType",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastNameAr",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastNameEn",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdOrIqama",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "UserInvitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "VirtualAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedWalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualAccounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VirtualAccounts_AccountNumber",
                table: "VirtualAccounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VirtualAccounts_Iban",
                table: "VirtualAccounts",
                column: "Iban",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VirtualAccounts");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "VirtualIban",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "VirtualIban",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "City",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "FirstNameAr",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "FirstNameEn",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "IdType",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "LastNameAr",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "LastNameEn",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "NationalIdOrIqama",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "UserInvitations");
        }
    }
}
