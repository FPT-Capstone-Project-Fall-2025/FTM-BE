using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class RemoveTitleAndCommentReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove Title column from PostComments
            migrationBuilder.DropColumn(
                name: "Title",
                table: "PostComments");

            // Remove PostCommentId column from PostReactions
            migrationBuilder.DropColumn(
                name: "PostCommentId",
                table: "PostReactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back Title column to PostComments
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "PostComments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // Add back PostCommentId column to PostReactions
            migrationBuilder.AddColumn<Guid>(
                name: "PostCommentId",
                table: "PostReactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostCommentId",
                table: "PostReactions",
                column: "PostCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_PostComments_PostCommentId",
                table: "PostReactions",
                column: "PostCommentId",
                principalTable: "PostComments",
                principalColumn: "Id");
        }
    }
}
