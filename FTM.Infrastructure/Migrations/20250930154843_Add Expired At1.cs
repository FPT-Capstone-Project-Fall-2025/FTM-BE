using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FTM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiredAt1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1532c39b-cfe9-4113-b5ba-1c829ebf9e27"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("69ce604f-a5ad-4ee7-8da7-afbcf8f62e47"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("681b430e-2fdf-42fa-adb3-b2c8a579d523"), "94a04417-c31b-42fb-acf8-7eaa6435de56", "Admin", "ADMIN" },
                    { new Guid("86423fa4-a161-4487-b3f9-44c3cbd60c1e"), "5d30130c-d21e-4fcf-8d2c-12d58818b0af", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("681b430e-2fdf-42fa-adb3-b2c8a579d523"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("86423fa4-a161-4487-b3f9-44c3cbd60c1e"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1532c39b-cfe9-4113-b5ba-1c829ebf9e27"), "7c3efeec-725e-4b34-8bc0-deb331d75673", "Admin", "ADMIN" },
                    { new Guid("69ce604f-a5ad-4ee7-8da7-afbcf8f62e47"), "76bd8e2f-6a3d-4d79-ab11-fcf07b34e635", "User", "USER" }
                });
        }
    }
}
