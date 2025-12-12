using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddSensitiveFlagToAuditScopeDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Areas",
                schema: "ams",
                table: "AuditScopeDepartment",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "ams",
                table: "AuditScopeDepartment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SensitiveFlag",
                schema: "ams",
                table: "AuditScopeDepartment",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Areas",
                schema: "ams",
                table: "AuditScopeDepartment");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "ams",
                table: "AuditScopeDepartment");

            migrationBuilder.DropColumn(
                name: "SensitiveFlag",
                schema: "ams",
                table: "AuditScopeDepartment");
        }
    }
}
