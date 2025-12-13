using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddSensitiveAreaLevelAndUpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeptID",
                schema: "ams",
                table: "RootCause",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FindingID",
                schema: "ams",
                table: "RootCause",
                type: "uniqueidentifier",
                nullable: true);

            // Step 1: Add temporary column for CreatedBy as Guid
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy_Temp",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                type: "uniqueidentifier",
                nullable: true);

            // Step 2: Convert existing string CreatedBy to Guid (if any data exists)
            // Note: This assumes CreatedBy was storing Guid as string. 
            // If it was storing something else, you need to adjust this conversion logic.
            migrationBuilder.Sql(@"
                UPDATE ams.DepartmentSensitiveArea
                SET CreatedBy_Temp = TRY_CAST(CreatedBy AS uniqueidentifier)
                WHERE CreatedBy IS NOT NULL AND TRY_CAST(CreatedBy AS uniqueidentifier) IS NOT NULL
            ");

            // Step 3: Drop old CreatedBy column
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            // Step 4: Rename temporary column to CreatedBy
            migrationBuilder.RenameColumn(
                name: "CreatedBy_Temp",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SensitiveAreaLevel",
                schema: "ams",
                columns: table => new
                {
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensitiveAreaLevel", x => x.Level);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RootCause_DeptID",
                schema: "ams",
                table: "RootCause",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_RootCause_FindingID",
                schema: "ams",
                table: "RootCause",
                column: "FindingID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentSensitiveArea_CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentSensitiveArea_Level",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "Level");

            migrationBuilder.AddForeignKey(
                name: "FK_DeptSensitiveArea_CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "CreatedBy",
                principalSchema: "auth",
                principalTable: "UserAccount",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_DeptSensitiveArea_Level",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "Level",
                principalSchema: "ams",
                principalTable: "SensitiveAreaLevel",
                principalColumn: "Level");

            migrationBuilder.AddForeignKey(
                name: "FK_RootCause_Dept",
                schema: "ams",
                table: "RootCause",
                column: "DeptID",
                principalSchema: "ams",
                principalTable: "Department",
                principalColumn: "DeptID");

            migrationBuilder.AddForeignKey(
                name: "FK_RootCause_Finding",
                schema: "ams",
                table: "RootCause",
                column: "FindingID",
                principalSchema: "ams",
                principalTable: "Finding",
                principalColumn: "FindingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeptSensitiveArea_CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            migrationBuilder.DropForeignKey(
                name: "FK_DeptSensitiveArea_Level",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            migrationBuilder.DropForeignKey(
                name: "FK_RootCause_Dept",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropForeignKey(
                name: "FK_RootCause_Finding",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropTable(
                name: "SensitiveAreaLevel",
                schema: "ams");

            migrationBuilder.DropIndex(
                name: "IX_RootCause_DeptID",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropIndex(
                name: "IX_RootCause_FindingID",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentSensitiveArea_CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentSensitiveArea_Level",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            migrationBuilder.DropColumn(
                name: "DeptID",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropColumn(
                name: "FindingID",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropColumn(
                name: "Level",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            // Revert CreatedBy back to string
            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                newName: "CreatedBy_Temp");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Convert Guid back to string (if needed)
            migrationBuilder.Sql(@"
                UPDATE ams.DepartmentSensitiveArea
                SET CreatedBy = CAST(CreatedBy_Temp AS nvarchar(100))
                WHERE CreatedBy_Temp IS NOT NULL
            ");

            migrationBuilder.DropColumn(
                name: "CreatedBy_Temp",
                schema: "ams",
                table: "DepartmentSensitiveArea");
        }
    }
}
