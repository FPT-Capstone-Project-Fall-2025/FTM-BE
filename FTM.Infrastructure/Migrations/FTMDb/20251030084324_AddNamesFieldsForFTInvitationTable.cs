using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddNamesFieldsForFTInvitationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FTMemberName",
                table: "FTInvitations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FTName",
                table: "FTInvitations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvitedName",
                table: "FTInvitations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InviterName",
                table: "FTInvitations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FTMemberName",
                table: "FTInvitations");

            migrationBuilder.DropColumn(
                name: "FTName",
                table: "FTInvitations");

            migrationBuilder.DropColumn(
                name: "InvitedName",
                table: "FTInvitations");

            migrationBuilder.DropColumn(
                name: "InviterName",
                table: "FTInvitations");
        }
    }
}
