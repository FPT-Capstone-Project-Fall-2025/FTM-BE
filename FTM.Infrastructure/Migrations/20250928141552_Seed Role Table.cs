using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FTM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("07b58e04-048f-42b1-855b-05b9b7aa439c"), "e6589af5-dbe2-4b45-8947-aa855e1f9db9", "User", "USER" },
                    { new Guid("30077366-9b95-4487-9345-ffec3fe61d04"), "9b569db5-3cf8-43d0-875d-273011e6267d", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("07b58e04-048f-42b1-855b-05b9b7aa439c"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("30077366-9b95-4487-9345-ffec3fe61d04"));
        }
    }
}
