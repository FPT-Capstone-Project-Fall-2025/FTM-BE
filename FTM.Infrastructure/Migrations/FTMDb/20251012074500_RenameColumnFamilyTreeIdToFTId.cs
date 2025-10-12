using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FTM.Infrastructure.Migrations.FTMDb
{
    /// <inheritdoc />
    public partial class RenameColumnFamilyTreeIdToFTId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename column using raw SQL - safe if column doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS(
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='FTMembers' AND column_name='FamilyTreeId'
                    ) THEN
                        ALTER TABLE ""FTMembers"" RENAME COLUMN ""FamilyTreeId"" TO ""FTId"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS(
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='FTMembers' AND column_name='FTId'
                    ) THEN
                        ALTER TABLE ""FTMembers"" RENAME COLUMN ""FTId"" TO ""FamilyTreeId"";
                    END IF;
                END $$;
            ");
        }
    }
}
