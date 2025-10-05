using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddTablessuchasFTMemberMEthnicMProvinceMReligionandMWard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SendOTPTrackings");

            migrationBuilder.CreateTable(
                name: "MEthnics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEthnics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MReligions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MReligions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FTMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Fullname = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EthnicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReligionId = table.Column<Guid>(type: "uuid", nullable: false),
                    WardId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProvinceId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDescription = table.Column<string>(type: "text", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "text", nullable: false),
                    IdentificationType = table.Column<string>(type: "text", nullable: false),
                    IsDeath = table.Column<bool>(type: "boolean", nullable: true),
                    DeathDescription = table.Column<string>(type: "text", nullable: false),
                    DeathDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BurialAddress = table.Column<string>(type: "text", nullable: false),
                    BurialWardId = table.Column<Guid>(type: "uuid", nullable: false),
                    BurialProvinceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrivacyData = table.Column<string>(type: "text", nullable: false),
                    IsRoot = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTMembers_MEthnics_EthnicId",
                        column: x => x.EthnicId,
                        principalTable: "MEthnics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTMembers_MReligions_ReligionId",
                        column: x => x.ReligionId,
                        principalTable: "MReligions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTMembers_MWards_BurialWardId",
                        column: x => x.BurialWardId,
                        principalTable: "MWards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTMembers_MWards_WardId",
                        column: x => x.WardId,
                        principalTable: "MWards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTMembers_Mprovinces_BurialProvinceId",
                        column: x => x.BurialProvinceId,
                        principalTable: "Mprovinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTMembers_Mprovinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Mprovinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_BurialProvinceId",
                table: "FTMembers",
                column: "BurialProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_BurialWardId",
                table: "FTMembers",
                column: "BurialWardId");

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_EthnicId",
                table: "FTMembers",
                column: "EthnicId");

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_ProvinceId",
                table: "FTMembers",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_ReligionId",
                table: "FTMembers",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_FTMembers_WardId",
                table: "FTMembers",
                column: "WardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FTMembers");

            migrationBuilder.DropTable(
                name: "MEthnics");

            migrationBuilder.DropTable(
                name: "MReligions");

            migrationBuilder.CreateTable(
                name: "SendOTPTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    RemoteIpAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendOTPTrackings", x => x.Id);
                });
        }
    }
}
