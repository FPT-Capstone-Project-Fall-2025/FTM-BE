using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddCampaignDonationsAndExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTFundCampaigns_FTFunds_FTFundId",
                table: "FTFundCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_FTFundCampaigns_FTMembers_CampaignManagerId",
                table: "FTFundCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_FTFundDonations_FTFundCampaigns_CampaignId",
                table: "FTFundDonations");

            migrationBuilder.DropForeignKey(
                name: "FK_FTFundExpenses_FTFundCampaigns_CampaignId",
                table: "FTFundExpenses");

            migrationBuilder.DropColumn(
                name: "MediaAttachments",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "OrganizerContact",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "OrganizerName",
                table: "FTFundCampaigns");

            migrationBuilder.RenameColumn(
                name: "FTFundId",
                table: "FTFundCampaigns",
                newName: "FTId");

            migrationBuilder.RenameColumn(
                name: "CurrentMoney",
                table: "FTFundCampaigns",
                newName: "CurrentBalance");

            migrationBuilder.RenameIndex(
                name: "IX_FTFundCampaigns_FTFundId",
                table: "FTFundCampaigns",
                newName: "IX_FTFundCampaigns_FTId");

            migrationBuilder.AddColumn<string>(
                name: "FundManagers",
                table: "FTFunds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FTFunds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "CampaignManagerId",
                table: "FTFundCampaigns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CampaignDescription",
                table: "FTFundCampaigns",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "FTFundCampaigns",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FTFundCampaigns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "FTFundCampaigns",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FTCampaignDonations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    FTMemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DonationAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    DonorNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PayOSOrderCode = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ConfirmedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ConfirmationNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTCampaignDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTCampaignDonations_FTFundCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "FTFundCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTCampaignDonations_FTMembers_ConfirmedBy",
                        column: x => x.ConfirmedBy,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTCampaignDonations_FTMembers_FTMemberId",
                        column: x => x.FTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FTCampaignExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorizedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExpenseDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExpenseAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    ExpenseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Recipient = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    ReceiptImages = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ApprovalStatus = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTCampaignExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTCampaignExpenses_FTFundCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "FTFundCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTCampaignExpenses_FTMembers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTCampaignExpenses_FTMembers_AuthorizedBy",
                        column: x => x.AuthorizedBy,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTCampaignDonations_CampaignId",
                table: "FTCampaignDonations",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_FTCampaignDonations_ConfirmedBy",
                table: "FTCampaignDonations",
                column: "ConfirmedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FTCampaignDonations_FTMemberId",
                table: "FTCampaignDonations",
                column: "FTMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FTCampaignExpenses_ApprovedBy",
                table: "FTCampaignExpenses",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FTCampaignExpenses_AuthorizedBy",
                table: "FTCampaignExpenses",
                column: "AuthorizedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FTCampaignExpenses_CampaignId",
                table: "FTCampaignExpenses",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundCampaigns_FTMembers_CampaignManagerId",
                table: "FTFundCampaigns",
                column: "CampaignManagerId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundCampaigns_FamilyTrees_FTId",
                table: "FTFundCampaigns",
                column: "FTId",
                principalTable: "FamilyTrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundDonations_FTFundCampaigns_CampaignId",
                table: "FTFundDonations",
                column: "CampaignId",
                principalTable: "FTFundCampaigns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundExpenses_FTFundCampaigns_CampaignId",
                table: "FTFundExpenses",
                column: "CampaignId",
                principalTable: "FTFundCampaigns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTFundCampaigns_FTMembers_CampaignManagerId",
                table: "FTFundCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_FTFundCampaigns_FamilyTrees_FTId",
                table: "FTFundCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_FTFundDonations_FTFundCampaigns_CampaignId",
                table: "FTFundDonations");

            migrationBuilder.DropForeignKey(
                name: "FK_FTFundExpenses_FTFundCampaigns_CampaignId",
                table: "FTFundExpenses");

            migrationBuilder.DropTable(
                name: "FTCampaignDonations");

            migrationBuilder.DropTable(
                name: "FTCampaignExpenses");

            migrationBuilder.DropColumn(
                name: "FundManagers",
                table: "FTFunds");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FTFunds");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FTFundCampaigns");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "FTFundCampaigns");

            migrationBuilder.RenameColumn(
                name: "FTId",
                table: "FTFundCampaigns",
                newName: "FTFundId");

            migrationBuilder.RenameColumn(
                name: "CurrentBalance",
                table: "FTFundCampaigns",
                newName: "CurrentMoney");

            migrationBuilder.RenameIndex(
                name: "IX_FTFundCampaigns_FTId",
                table: "FTFundCampaigns",
                newName: "IX_FTFundCampaigns_FTFundId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CampaignManagerId",
                table: "FTFundCampaigns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CampaignDescription",
                table: "FTFundCampaigns",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "MediaAttachments",
                table: "FTFundCampaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerContact",
                table: "FTFundCampaigns",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerName",
                table: "FTFundCampaigns",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundCampaigns_FTFunds_FTFundId",
                table: "FTFundCampaigns",
                column: "FTFundId",
                principalTable: "FTFunds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundCampaigns_FTMembers_CampaignManagerId",
                table: "FTFundCampaigns",
                column: "CampaignManagerId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundDonations_FTFundCampaigns_CampaignId",
                table: "FTFundDonations",
                column: "CampaignId",
                principalTable: "FTFundCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FTFundExpenses_FTFundCampaigns_CampaignId",
                table: "FTFundExpenses",
                column: "CampaignId",
                principalTable: "FTFundCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
