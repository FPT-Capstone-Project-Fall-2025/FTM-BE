using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddHonorBoards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicHonors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GPMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    FamilyTreeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementTitle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    InstitutionName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DegreeOrCertificate = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    YearOfAchievement = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDisplayed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicHonors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicHonors_FTMembers_GPMemberId",
                        column: x => x.GPMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicHonors_FamilyTrees_FamilyTreeId",
                        column: x => x.FamilyTreeId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CareerHonors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GPMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    FamilyTreeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementTitle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Position = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    YearOfAchievement = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDisplayed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerHonors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareerHonors_FTMembers_GPMemberId",
                        column: x => x.GPMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CareerHonors_FamilyTrees_FamilyTreeId",
                        column: x => x.FamilyTreeId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_FamilyTreeId",
                table: "AcademicHonors",
                column: "FamilyTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_GPMemberId",
                table: "AcademicHonors",
                column: "GPMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_IsDisplayed",
                table: "AcademicHonors",
                column: "IsDisplayed");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_YearOfAchievement",
                table: "AcademicHonors",
                column: "YearOfAchievement");

            migrationBuilder.CreateIndex(
                name: "IX_CareerHonors_FamilyTreeId",
                table: "CareerHonors",
                column: "FamilyTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_CareerHonors_GPMemberId",
                table: "CareerHonors",
                column: "GPMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CareerHonors_IsDisplayed",
                table: "CareerHonors",
                column: "IsDisplayed");

            migrationBuilder.CreateIndex(
                name: "IX_CareerHonors_YearOfAchievement",
                table: "CareerHonors",
                column: "YearOfAchievement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicHonors");

            migrationBuilder.DropTable(
                name: "CareerHonors");
        }
    }
}
