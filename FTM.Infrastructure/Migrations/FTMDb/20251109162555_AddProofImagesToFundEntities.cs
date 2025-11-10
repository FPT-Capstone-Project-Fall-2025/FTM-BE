using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddProofImagesToFundEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptImages",
                table: "FTFundExpenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofImages",
                table: "FTFundDonations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofImages",
                table: "FTCampaignDonations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptImages",
                table: "FTFundExpenses");

            migrationBuilder.DropColumn(
                name: "ProofImages",
                table: "FTFundDonations");

            migrationBuilder.DropColumn(
                name: "ProofImages",
                table: "FTCampaignDonations");
        }
    }
}
