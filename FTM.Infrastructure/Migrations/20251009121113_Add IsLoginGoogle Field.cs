using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FTM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLoginGoogleField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("681b430e-2fdf-42fa-adb3-b2c8a579d523"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("86423fa4-a161-4487-b3f9-44c3cbd60c1e"));

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleLogin",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("454c2507-53a5-44a5-8a09-5ac6c41aab50"), "84f8c030-4071-47ce-9f32-52a5a73bac37", "User", "USER" },
                    { new Guid("b94314fb-f89b-46f7-b9bf-14b013db16b7"), "5ab10cbc-8f7b-4533-ba7b-ec0f9aefc5f4", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("454c2507-53a5-44a5-8a09-5ac6c41aab50"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b94314fb-f89b-46f7-b9bf-14b013db16b7"));

            migrationBuilder.DropColumn(
                name: "IsGoogleLogin",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("681b430e-2fdf-42fa-adb3-b2c8a579d523"), "94a04417-c31b-42fb-acf8-7eaa6435de56", "Admin", "ADMIN" },
                    { new Guid("86423fa4-a161-4487-b3f9-44c3cbd60c1e"), "5d30130c-d21e-4fcf-8d2c-12d58818b0af", "User", "USER" }
                });
        }
    }
}
