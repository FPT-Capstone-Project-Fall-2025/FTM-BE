using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddFTFamilyEventTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FTFamilyEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Recurrence = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FTId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReferenceEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LocationName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RecurrenceEndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsLunar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFamilyEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFamilyEvents_FamilyTrees_FTId",
                        column: x => x.FTId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FTFamilyEventFTs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTFamilyEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    FTId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFamilyEventFTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFamilyEventFTs_FTFamilyEvents_FTFamilyEventId",
                        column: x => x.FTFamilyEventId,
                        principalTable: "FTFamilyEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FTFamilyEventFTs_FamilyTrees_FTId",
                        column: x => x.FTId,
                        principalTable: "FamilyTrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FTFamilyEventMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FTFamilyEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    FTMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTFamilyEventMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTFamilyEventMembers_FTFamilyEvents_FTFamilyEventId",
                        column: x => x.FTFamilyEventId,
                        principalTable: "FTFamilyEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FTFamilyEventMembers_FTMembers_FTMemberId",
                        column: x => x.FTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTFamilyEventFTs_FTFamilyEventId_FTId",
                table: "FTFamilyEventFTs",
                columns: new[] { "FTFamilyEventId", "FTId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FTFamilyEventFTs_FTId",
                table: "FTFamilyEventFTs",
                column: "FTId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFamilyEventMembers_FTFamilyEventId_FTMemberId",
                table: "FTFamilyEventMembers",
                columns: new[] { "FTFamilyEventId", "FTMemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FTFamilyEventMembers_FTMemberId",
                table: "FTFamilyEventMembers",
                column: "FTMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FTFamilyEvents_FTId",
                table: "FTFamilyEvents",
                column: "FTId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FTFamilyEventFTs");

            migrationBuilder.DropTable(
                name: "FTFamilyEventMembers");

            migrationBuilder.DropTable(
                name: "FTFamilyEvents");
        }
    }
}
