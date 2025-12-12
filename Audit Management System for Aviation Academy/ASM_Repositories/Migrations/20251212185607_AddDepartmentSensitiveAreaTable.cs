using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentSensitiveAreaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentSensitiveArea",
                schema: "ams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    DeptID = table.Column<int>(type: "int", nullable: false),
                    SensitiveAreas = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    DefaultNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__3214EC07A1B2C3D4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeptSensitiveArea_Dept",
                        column: x => x.DeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ_DeptSensitiveArea_DeptId",
                schema: "ams",
                table: "DepartmentSensitiveArea",
                column: "DeptID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentSensitiveArea",
                schema: "ams");
        }
    }
}
