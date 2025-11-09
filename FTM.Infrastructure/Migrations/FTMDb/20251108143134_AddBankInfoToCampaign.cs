using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddBankInfoToCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountHolderName",
                table: "FTFundCampaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "FTFundCampaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "FTFundCampaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "FTFundCampaigns",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountHolderName",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "FTFundCampaigns");
        }
    }
}
