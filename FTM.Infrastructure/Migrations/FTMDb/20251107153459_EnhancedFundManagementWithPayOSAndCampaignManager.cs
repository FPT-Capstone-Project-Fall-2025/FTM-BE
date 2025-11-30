using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class EnhancedFundManagementWithPayOSAndCampaignManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_FTMembers_GPMemberId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_FTMembers_GPMemberId",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_FTMembers_GPMemberId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "GPMemberId",
                table: "Posts",
                newName: "FTMemberId");

            migrationBuilder.RenameColumn(
                name: "GPId",
                table: "Posts",
                newName: "FTId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_GPMemberId",
                table: "Posts",
                newName: "IX_Posts_FTMemberId");

            migrationBuilder.RenameColumn(
                name: "GPMemberId",
                table: "PostReactions",
                newName: "FTMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_PostReactions_PostId_GPMemberId",
                table: "PostReactions",
                newName: "IX_PostReactions_PostId_FTMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_PostReactions_GPMemberId",
                table: "PostReactions",
                newName: "IX_PostReactions_FTMemberId");

            migrationBuilder.RenameColumn(
                name: "GPMemberId",
                table: "PostComments",
                newName: "FTMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComments_GPMemberId",
                table: "PostComments",
                newName: "IX_PostComments_FTMemberId");

            migrationBuilder.CreateTable(
                name: "FTFunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentMoney = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    FundNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FundName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFunds_FamilyTrees_FTId",
                        column: x => x.FTId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FTFundCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTFundId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CampaignDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OrganizerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrganizerContact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CampaignManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FundGoal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrentMoney = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    MediaAttachments = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFundCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFundCampaigns_FTFunds_FTFundId",
                        column: x => x.FTFundId,
                        principalTable: "FTFunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTFundCampaigns_FTMembers_CampaignManagerId",
                        column: x => x.CampaignManagerId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FTFundDonations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTFundId = table.Column<Guid>(type: "uuid", nullable: false),
                    FTMemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonationMoney = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DonorName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ConfirmedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ConfirmationNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFundDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFundDonations_FTFundCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "FTFundCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FTFundDonations_FTFunds_FTFundId",
                        column: x => x.FTFundId,
                        principalTable: "FTFunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FTFundDonations_FTMembers_ConfirmedBy",
                        column: x => x.ConfirmedBy,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FTFundDonations_FTMembers_FTMemberId",
                        column: x => x.FTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FTFundExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTFundId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpenseAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ExpenseDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ExpenseEvent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Recipient = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovalFeedback = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PlannedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFundExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFundExpenses_FTFundCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "FTFundCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FTFundExpenses_FTFunds_FTFundId",
                        column: x => x.FTFundId,
                        principalTable: "FTFunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FTFundExpenses_FTMembers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FTPayOSTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PayOSTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DonationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PayOSStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WebhookData = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTPayOSTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTPayOSTransactions_FTFundDonations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "FTFundDonations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_CampaignManagerId",
                table: "FTFundCampaigns",
                column: "CampaignManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_CreatedOn",
                table: "FTFundCampaigns",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_EndDate",
                table: "FTFundCampaigns",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_FTFundId",
                table: "FTFundCampaigns",
                column: "FTFundId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_IsDeleted",
                table: "FTFundCampaigns",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_StartDate",
                table: "FTFundCampaigns",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundCampaigns_Status",
                table: "FTFundCampaigns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_CampaignId",
                table: "FTFundDonations",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_ConfirmedBy",
                table: "FTFundDonations",
                column: "ConfirmedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_CreatedOn",
                table: "FTFundDonations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_FTFundId",
                table: "FTFundDonations",
                column: "FTFundId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_FTMemberId",
                table: "FTFundDonations",
                column: "FTMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_PaymentMethod",
                table: "FTFundDonations",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundDonations_Status",
                table: "FTFundDonations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundExpenses_ApprovedBy",
                table: "FTFundExpenses",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundExpenses_CampaignId",
                table: "FTFundExpenses",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundExpenses_CreatedOn",
                table: "FTFundExpenses",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundExpenses_FTFundId",
                table: "FTFundExpenses",
                column: "FTFundId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundExpenses_PlannedDate",
                table: "FTFundExpenses",
                column: "PlannedDate");

            migrationBuilder.CreateIndex(
                name: "IX_FTFundExpenses_Status",
                table: "FTFundExpenses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FTFunds_CreatedOn",
                table: "FTFunds",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FTFunds_FTId",
                table: "FTFunds",
                column: "FTId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFunds_IsDeleted",
                table: "FTFunds",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FTPayOSTransactions_CreatedOn",
                table: "FTPayOSTransactions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FTPayOSTransactions_DonationId",
                table: "FTPayOSTransactions",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_FTPayOSTransactions_OrderCode",
                table: "FTPayOSTransactions",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FTPayOSTransactions_PayOSStatus",
                table: "FTPayOSTransactions",
                column: "PayOSStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FTPayOSTransactions_PayOSTransactionId",
                table: "FTPayOSTransactions",
                column: "PayOSTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_FTMembers_FTMemberId",
                table: "PostComments",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_FTMembers_FTMemberId",
                table: "PostReactions",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_FTMembers_FTMemberId",
                table: "Posts",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_FTMembers_FTMemberId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_FTMembers_FTMemberId",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_FTMembers_FTMemberId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "FTFundExpenses");

            migrationBuilder.DropTable(
                name: "FTPayOSTransactions");

            migrationBuilder.DropTable(
                name: "FTFundDonations");

            migrationBuilder.DropTable(
                name: "FTFundCampaigns");

            migrationBuilder.DropTable(
                name: "FTFunds");

            migrationBuilder.RenameColumn(
                name: "FTMemberId",
                table: "Posts",
                newName: "GPMemberId");

            migrationBuilder.RenameColumn(
                name: "FTId",
                table: "Posts",
                newName: "GPId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_FTMemberId",
                table: "Posts",
                newName: "IX_Posts_GPMemberId");

            migrationBuilder.RenameColumn(
                name: "FTMemberId",
                table: "PostReactions",
                newName: "GPMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_PostReactions_PostId_FTMemberId",
                table: "PostReactions",
                newName: "IX_PostReactions_PostId_GPMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_PostReactions_FTMemberId",
                table: "PostReactions",
                newName: "IX_PostReactions_GPMemberId");

            migrationBuilder.RenameColumn(
                name: "FTMemberId",
                table: "PostComments",
                newName: "GPMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComments_FTMemberId",
                table: "PostComments",
                newName: "IX_PostComments_GPMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_FTMembers_GPMemberId",
                table: "PostComments",
                column: "GPMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_FTMembers_GPMemberId",
                table: "PostReactions",
                column: "GPMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_FTMembers_GPMemberId",
                table: "Posts",
                column: "GPMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
