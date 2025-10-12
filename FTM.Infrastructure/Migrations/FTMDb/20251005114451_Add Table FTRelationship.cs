using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddTableFTRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTMembers_Mprovinces_BurialProvinceId",
                table: "FTMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_FTMembers_Mprovinces_ProvinceId",
                table: "FTMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mprovinces",
                table: "Mprovinces");

            migrationBuilder.RenameTable(
                name: "Mprovinces",
                newName: "MProvinces");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MProvinces",
                table: "MProvinces",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FTRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    FromFTMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromFTMemberPartnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToFTMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryCode = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FTRelationships_FTMembers_FromFTMemberId",
                        column: x => x.FromFTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTRelationships_FTMembers_FromFTMemberPartnerId",
                        column: x => x.FromFTMemberPartnerId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FTRelationships_FTMembers_ToFTMemberId",
                        column: x => x.ToFTMemberId,
                        principalTable: "FTMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTRelationships_FromFTMemberId",
                table: "FTRelationships",
                column: "FromFTMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FTRelationships_FromFTMemberPartnerId",
                table: "FTRelationships",
                column: "FromFTMemberPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FTRelationships_ToFTMemberId",
                table: "FTRelationships",
                column: "ToFTMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_FTMembers_MProvinces_BurialProvinceId",
                table: "FTMembers",
                column: "BurialProvinceId",
                principalTable: "MProvinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTMembers_MProvinces_ProvinceId",
                table: "FTMembers",
                column: "ProvinceId",
                principalTable: "MProvinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTMembers_MProvinces_BurialProvinceId",
                table: "FTMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_FTMembers_MProvinces_ProvinceId",
                table: "FTMembers");

            migrationBuilder.DropTable(
                name: "FTRelationships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MProvinces",
                table: "MProvinces");

            migrationBuilder.RenameTable(
                name: "MProvinces",
                newName: "Mprovinces");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mprovinces",
                table: "Mprovinces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FTMembers_Mprovinces_BurialProvinceId",
                table: "FTMembers",
                column: "BurialProvinceId",
                principalTable: "Mprovinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FTMembers_Mprovinces_ProvinceId",
                table: "FTMembers",
                column: "ProvinceId",
                principalTable: "Mprovinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
