using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddIsDivorcedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDivorced",
                table: "FTMembers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDivorced",
                table: "FTMembers");
        }
    }
}
