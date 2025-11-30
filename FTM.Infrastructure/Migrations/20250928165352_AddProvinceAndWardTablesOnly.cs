using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProvinceAndWardTablesOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MWard_WardId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Mprovince_ProvinceId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MWard",
                table: "MWard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mprovince",
                table: "Mprovince");

            migrationBuilder.RenameTable(
                name: "MWard",
                newName: "MWards");

            migrationBuilder.RenameTable(
                name: "Mprovince",
                newName: "Mprovinces");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MWards",
                table: "MWards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mprovinces",
                table: "Mprovinces",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MWards_Code",
                table: "MWards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mprovinces_Code",
                table: "Mprovinces",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MWards_WardId",
                table: "AspNetUsers",
                column: "WardId",
                principalTable: "MWards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Mprovinces_ProvinceId",
                table: "AspNetUsers",
                column: "ProvinceId",
                principalTable: "Mprovinces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MWards_WardId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Mprovinces_ProvinceId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MWards",
                table: "MWards");

            migrationBuilder.DropIndex(
                name: "IX_MWards_Code",
                table: "MWards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mprovinces",
                table: "Mprovinces");

            migrationBuilder.DropIndex(
                name: "IX_Mprovinces_Code",
                table: "Mprovinces");

            migrationBuilder.RenameTable(
                name: "MWards",
                newName: "MWard");

            migrationBuilder.RenameTable(
                name: "Mprovinces",
                newName: "Mprovince");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MWard",
                table: "MWard",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mprovince",
                table: "Mprovince",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MWard_WardId",
                table: "AspNetUsers",
                column: "WardId",
                principalTable: "MWard",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Mprovince_ProvinceId",
                table: "AspNetUsers",
                column: "ProvinceId",
                principalTable: "Mprovince",
                principalColumn: "Id");
        }
    }
}
