using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessGrantTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessGrant",
                schema: "ams",
                columns: table => new
                {
                    GrantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeptID = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TtlMinutes = table.Column<int>(type: "int", nullable: false),
                    QrToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QrUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AccessGr__90E9FF8A12345678", x => x.GrantID);
                    table.ForeignKey(
                        name: "FK__AccessGra__Audit__AccessGrant",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                    table.ForeignKey(
                        name: "FK__AccessGra__Audit__AccessGrant2",
                        column: x => x.AuditorID,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__AccessGra__DeptI__AccessGrant",
                        column: x => x.DeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessGrant_AuditID",
                schema: "ams",
                table: "AccessGrant",
                column: "AuditID");

            migrationBuilder.CreateIndex(
                name: "IX_AccessGrant_AuditorID",
                schema: "ams",
                table: "AccessGrant",
                column: "AuditorID");

            migrationBuilder.CreateIndex(
                name: "IX_AccessGrant_DeptID",
                schema: "ams",
                table: "AccessGrant",
                column: "DeptID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessGrant",
                schema: "ams");
        }
    }
}
