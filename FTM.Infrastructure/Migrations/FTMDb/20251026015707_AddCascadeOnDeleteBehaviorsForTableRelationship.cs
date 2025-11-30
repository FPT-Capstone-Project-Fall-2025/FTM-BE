using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddCascadeOnDeleteBehaviorsForTableRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberId",
                table: "FTRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberPartnerId",
                table: "FTRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_FTRelationships_FTMembers_ToFTMemberId",
                table: "FTRelationships");

            migrationBuilder.AddForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberId",
                table: "FTRelationships",
                column: "FromFTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberPartnerId",
                table: "FTRelationships",
                column: "FromFTMemberPartnerId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FTRelationships_FTMembers_ToFTMemberId",
                table: "FTRelationships",
                column: "ToFTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberId",
                table: "FTRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberPartnerId",
                table: "FTRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_FTRelationships_FTMembers_ToFTMemberId",
                table: "FTRelationships");

            migrationBuilder.AddForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberId",
                table: "FTRelationships",
                column: "FromFTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTRelationships_FTMembers_FromFTMemberPartnerId",
                table: "FTRelationships",
                column: "FromFTMemberPartnerId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTRelationships_FTMembers_ToFTMemberId",
                table: "FTRelationships",
                column: "ToFTMemberId",
                principalTable: "FTMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
