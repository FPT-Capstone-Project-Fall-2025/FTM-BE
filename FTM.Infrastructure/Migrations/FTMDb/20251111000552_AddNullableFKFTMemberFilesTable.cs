using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    public partial class AddNullableFKFTMemberFilesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop FK first
            migrationBuilder.DropForeignKey(
                name: "FK_FTMemberFiles_FTMembers_FTMemberId",
                table: "FTMemberFiles");

            // 2. Alter column to nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "FTMemberId",
                table: "FTMemberFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 3. Re-add FK with desired delete behavior
            migrationBuilder.AddForeignKey(
                name: "FK_FTMemberFiles_FTMembers_FTMemberId",
                table: "FTMemberFiles",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // matches your config
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse order:

            migrationBuilder.DropForeignKey(
                name: "FK_FTMemberFiles_FTMembers_FTMemberId",
                table: "FTMemberFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "FTMemberId",
                table: "FTMemberFiles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FTMemberFiles_FTMembers_FTMemberId",
                table: "FTMemberFiles",
                column: "FTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
