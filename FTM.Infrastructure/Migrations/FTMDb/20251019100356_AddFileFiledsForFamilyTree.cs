using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class AddFileFiledsForFamilyTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "FamilyTrees",
                newName: "FilePath");

            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "FamilyTrees",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "FamilyTrees");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "FamilyTrees",
                newName: "Picture");
        }
    }
}
