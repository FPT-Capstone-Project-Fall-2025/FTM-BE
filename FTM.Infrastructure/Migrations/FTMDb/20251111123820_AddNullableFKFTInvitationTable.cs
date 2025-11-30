using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    public partial class AddNullableFKFTInvitationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop old FK constraint
            migrationBuilder.DropForeignKey(
                name: "FK_FTInvitations_FTMembers_FTMemberId",
                table: "FTInvitations");

            // 2. Make FTMemberId nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "FTMemberId",
                table: "FTInvitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 3. Make FTMemberName nullable
            migrationBuilder.AlterColumn<string>(
                name: "FTMemberName",
                table: "FTInvitations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // 4. Re-add FK with cascade delete (or Restrict — up to your logic)
            migrationBuilder.AddForeignKey(
                name: "FK_FTInvitations_FTMembers_FTMemberId",
                table: "FTInvitations",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTInvitations_FTMembers_FTMemberId",
                table: "FTInvitations");

            migrationBuilder.AlterColumn<Guid>(
                name: "FTMemberId",
                table: "FTInvitations",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FTMemberName",
                table: "FTInvitations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FTInvitations_FTMembers_FTMemberId",
                table: "FTInvitations",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
