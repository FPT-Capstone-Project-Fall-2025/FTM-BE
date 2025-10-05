using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddTablessuchasFTMembeFileandFamilyTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FTId",
                table: "FTMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "FamilyTrees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    GPModeCode = table.Column<int>(type: "integer", nullable: true),
                    IsShareEnable = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyTrees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FTMemberFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    FTMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UploadedBy = table.Column<string>(type: "text", nullable: false),
                    UploadedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTMemberFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTMemberFiles_FTMembers_FTMemberId",
                        column: x => x.FTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_FTId",
                table: "FTMembers",
                column: "FTId");

            migrationBuilder.CreateIndex(
                name: "IX_FTMemberFiles_FTMemberId",
                table: "FTMemberFiles",
                column: "FTMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_FTMembers_FamilyTrees_FTId",
                table: "FTMembers",
                column: "FTId",
                principalTable: "FamilyTrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTMembers_FamilyTrees_FTId",
                table: "FTMembers");

            migrationBuilder.DropTable(
                name: "FamilyTrees");

            migrationBuilder.DropTable(
                name: "FTMemberFiles");

            migrationBuilder.DropIndex(
                name: "IX_FTMembers_FTId",
                table: "FTMembers");

            migrationBuilder.DropColumn(
                name: "FTId",
                table: "FTMembers");
        }
    }
}
