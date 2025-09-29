using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FTM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiredAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("07b58e04-048f-42b1-855b-05b9b7aa439c"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("30077366-9b95-4487-9345-ffec3fe61d04"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiredAt",
                table: "UserRefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1532c39b-cfe9-4113-b5ba-1c829ebf9e27"), "7c3efeec-725e-4b34-8bc0-deb331d75673", "Admin", "ADMIN" },
                    { new Guid("69ce604f-a5ad-4ee7-8da7-afbcf8f62e47"), "76bd8e2f-6a3d-4d79-ab11-fcf07b34e635", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1532c39b-cfe9-4113-b5ba-1c829ebf9e27"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("69ce604f-a5ad-4ee7-8da7-afbcf8f62e47"));

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "UserRefreshTokens");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("07b58e04-048f-42b1-855b-05b9b7aa439c"), "e6589af5-dbe2-4b45-8947-aa855e1f9db9", "User", "USER" },
                    { new Guid("30077366-9b95-4487-9345-ffec3fe61d04"), "9b569db5-3cf8-43d0-875d-273011e6267d", "Admin", "ADMIN" }
                });
        }
    }
}
