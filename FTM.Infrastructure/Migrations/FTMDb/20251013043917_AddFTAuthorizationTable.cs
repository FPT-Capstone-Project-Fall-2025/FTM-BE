using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddFTAuthorizationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FTAuthorizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTId = table.Column<Guid>(type: "uuid", nullable: false),
                    FTMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MethodCode = table.Column<string>(type: "text", nullable: false),
                    FeatureCode = table.Column<string>(type: "text", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTAuthorizations_FTMembers_FTMemberId",
                        column: x => x.FTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FTAuthorizations_FamilyTrees_FTId",
                        column: x => x.FTId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTAuthorizations_FTId",
                table: "FTAuthorizations",
                column: "FTId");

            migrationBuilder.CreateIndex(
                name: "IX_FTAuthorizations_FTMemberId",
                table: "FTAuthorizations",
                column: "FTMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FTAuthorizations");
        }
    }
}
