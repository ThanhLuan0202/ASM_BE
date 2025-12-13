using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndexFromDepartmentSensitiveArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_DeptSensitiveArea_DeptId",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentSensitiveArea_DeptID",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "DeptID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DepartmentSensitiveArea_DeptID",
                schema: "ams",
                table: "DepartmentSensitiveArea");

            migrationBuilder.CreateIndex(
                name: "UQ_DeptSensitiveArea_DeptId",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "DeptID",
                unique: true);
        }
    }
}
