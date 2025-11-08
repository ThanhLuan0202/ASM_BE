using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Update_auditlogFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__UserAcco__1788CCACF4537956",
                schema: "auth",
                table: "UserAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK__RootCaus__EAE5E40E141DB7EE",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8A2B6161FA3AFAF9",
                schema: "ams",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ReportRe__1609F931C53C5848",
                schema: "ams",
                table: "ReportRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Notifica__20CF2E32D3D37E2C",
                schema: "ams",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK__FindingS__B9A531DF589A3323",
                schema: "ams",
                table: "FindingStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__FindingS__96F5CD158AB2DB08",
                schema: "ams",
                table: "FindingSeverity");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Finding__19D671C2547241BC",
                schema: "ams",
                table: "Finding");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Departme__04A19341CE4BC1D4",
                schema: "ams",
                table: "DepartmentHead");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Departme__0148818EC40042CD",
                schema: "ams",
                table: "Department");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Checklis__F87ADD07BBFA0B57",
                schema: "ams",
                table: "ChecklistTemplate");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Checklis__727E83EB1BF68FE1",
                schema: "ams",
                table: "ChecklistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditTea__7093F172CE7CC93A",
                schema: "ams",
                table: "AuditTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditSta__45D7E6365E6E9D5F",
                schema: "ams",
                table: "AuditStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditSco__405679D3A645CC18",
                schema: "ams",
                table: "AuditScopeDepartment");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditLog__5E5499A840991FB9",
                schema: "log",
                table: "AuditLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditDoc__3EF1888DFCF43C9F",
                schema: "ams",
                table: "AuditDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditCri__FE6ADB2DA611FA2C",
                schema: "ams",
                table: "AuditCriteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditChe__E37F18C209B145A9",
                schema: "ams",
                table: "AuditChecklistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditApp__1C6F87158CF157F6",
                schema: "ams",
                table: "AuditApproval");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Audit_Cr__6E998E0A225AF767",
                schema: "ams",
                table: "Audit_Criteria_Map");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Audit__A17F23B89C9D3A29",
                schema: "ams",
                table: "Audit");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Attachme__F21D4CA2922583A4",
                schema: "ams",
                table: "AttachmentEntityType");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Attachme__442C64DEEF5BB30F",
                schema: "ams",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ActionSt__7B3DC58EC9726C81",
                schema: "ams",
                table: "ActionStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Action__FFE3F4B991421ECC",
                schema: "ams",
                table: "Action");

            migrationBuilder.RenameIndex(
                name: "UQ__UserAcco__A9D1053420E1AC15",
                schema: "auth",
                table: "UserAccount",
                newName: "UQ__UserAcco__A9D10534D42B0AFC");

            migrationBuilder.RenameIndex(
                name: "UQ__Departme__D0300D45EA024CB1",
                schema: "ams",
                table: "DepartmentHead",
                newName: "UQ__Departme__D0300D454B9884C0");

            migrationBuilder.RenameIndex(
                name: "UQ__AuditTea__7007AF733DCB199C",
                schema: "ams",
                table: "AuditTeam",
                newName: "UQ__AuditTea__7007AF73218720A0");

            migrationBuilder.RenameColumn(
                name: "Data",
                schema: "log",
                table: "AuditLog",
                newName: "OldValue");

            migrationBuilder.RenameIndex(
                name: "UQ__AuditApp__592EA2A7366B9DD9",
                schema: "ams",
                table: "AuditApproval",
                newName: "UQ__AuditApp__592EA2A7168E30A8");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                schema: "log",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "log",
                table: "AuditLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__UserAcco__1788CCAC78E72EFF",
                schema: "auth",
                table: "UserAccount",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__RootCaus__EAE5E40EC3D9468A",
                schema: "ams",
                table: "RootCause",
                column: "RootCauseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8A2B6161E48C8F2C",
                schema: "ams",
                table: "Role",
                column: "RoleName");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ReportRe__1609F931CB3E84AE",
                schema: "ams",
                table: "ReportRequest",
                column: "ReportRequestID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Notifica__20CF2E32DACF1590",
                schema: "ams",
                table: "Notification",
                column: "NotificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__FindingS__B9A531DFE9BD16DD",
                schema: "ams",
                table: "FindingStatus",
                column: "FindingStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK__FindingS__96F5CD159FC0754C",
                schema: "ams",
                table: "FindingSeverity",
                column: "Severity");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Finding__19D671C271F1A8F1",
                schema: "ams",
                table: "Finding",
                column: "FindingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Departme__04A193417906A8B4",
                schema: "ams",
                table: "DepartmentHead",
                column: "DeptHeadID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Departme__0148818E083A3CEC",
                schema: "ams",
                table: "Department",
                column: "DeptID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Checklis__F87ADD07572BA702",
                schema: "ams",
                table: "ChecklistTemplate",
                column: "TemplateID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Checklis__727E83EB0D9B4F88",
                schema: "ams",
                table: "ChecklistItem",
                column: "ItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditTea__7093F172AF8F2CEE",
                schema: "ams",
                table: "AuditTeam",
                column: "AuditTeamID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditSta__45D7E636730CC5AF",
                schema: "ams",
                table: "AuditStatus",
                column: "AuditStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditSco__405679D3D34BF00B",
                schema: "ams",
                table: "AuditScopeDepartment",
                column: "AuditScopeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditLog__5E5499A86BF1FE20",
                schema: "log",
                table: "AuditLog",
                column: "LogID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditDoc__3EF1888DB890BF0C",
                schema: "ams",
                table: "AuditDocument",
                column: "DocID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditCri__FE6ADB2D1E0300D4",
                schema: "ams",
                table: "AuditCriteria",
                column: "CriteriaID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditChe__E37F18C217B7CDF3",
                schema: "ams",
                table: "AuditChecklistItem",
                column: "AuditItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditApp__1C6F87158FA8BD28",
                schema: "ams",
                table: "AuditApproval",
                column: "AuditApprovalID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Audit_Cr__6E998E0ABFC90124",
                schema: "ams",
                table: "Audit_Criteria_Map",
                columns: new[] { "AuditID", "CriteriaID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK__Audit__A17F23B854AA04E5",
                schema: "ams",
                table: "Audit",
                column: "AuditID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Attachme__F21D4CA2580E4F9D",
                schema: "ams",
                table: "AttachmentEntityType",
                column: "EntityType");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Attachme__442C64DE512B798F",
                schema: "ams",
                table: "Attachment",
                column: "AttachmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ActionSt__7B3DC58E4DAFA866",
                schema: "ams",
                table: "ActionStatus",
                column: "ActionStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Action__FFE3F4B9A4603102",
                schema: "ams",
                table: "Action",
                column: "ActionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__UserAcco__1788CCAC78E72EFF",
                schema: "auth",
                table: "UserAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK__RootCaus__EAE5E40EC3D9468A",
                schema: "ams",
                table: "RootCause");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8A2B6161E48C8F2C",
                schema: "ams",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ReportRe__1609F931CB3E84AE",
                schema: "ams",
                table: "ReportRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Notifica__20CF2E32DACF1590",
                schema: "ams",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK__FindingS__B9A531DFE9BD16DD",
                schema: "ams",
                table: "FindingStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__FindingS__96F5CD159FC0754C",
                schema: "ams",
                table: "FindingSeverity");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Finding__19D671C271F1A8F1",
                schema: "ams",
                table: "Finding");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Departme__04A193417906A8B4",
                schema: "ams",
                table: "DepartmentHead");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Departme__0148818E083A3CEC",
                schema: "ams",
                table: "Department");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Checklis__F87ADD07572BA702",
                schema: "ams",
                table: "ChecklistTemplate");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Checklis__727E83EB0D9B4F88",
                schema: "ams",
                table: "ChecklistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditTea__7093F172AF8F2CEE",
                schema: "ams",
                table: "AuditTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditSta__45D7E636730CC5AF",
                schema: "ams",
                table: "AuditStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditSco__405679D3D34BF00B",
                schema: "ams",
                table: "AuditScopeDepartment");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditLog__5E5499A86BF1FE20",
                schema: "log",
                table: "AuditLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditDoc__3EF1888DB890BF0C",
                schema: "ams",
                table: "AuditDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditCri__FE6ADB2D1E0300D4",
                schema: "ams",
                table: "AuditCriteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditChe__E37F18C217B7CDF3",
                schema: "ams",
                table: "AuditChecklistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AuditApp__1C6F87158FA8BD28",
                schema: "ams",
                table: "AuditApproval");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Audit_Cr__6E998E0ABFC90124",
                schema: "ams",
                table: "Audit_Criteria_Map");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Audit__A17F23B854AA04E5",
                schema: "ams",
                table: "Audit");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Attachme__F21D4CA2580E4F9D",
                schema: "ams",
                table: "AttachmentEntityType");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Attachme__442C64DE512B798F",
                schema: "ams",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ActionSt__7B3DC58E4DAFA866",
                schema: "ams",
                table: "ActionStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Action__FFE3F4B9A4603102",
                schema: "ams",
                table: "Action");

            migrationBuilder.DropColumn(
                name: "NewValue",
                schema: "log",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "log",
                table: "AuditLog");

            migrationBuilder.RenameIndex(
                name: "UQ__UserAcco__A9D10534D42B0AFC",
                schema: "auth",
                table: "UserAccount",
                newName: "UQ__UserAcco__A9D1053420E1AC15");

            migrationBuilder.RenameIndex(
                name: "UQ__Departme__D0300D454B9884C0",
                schema: "ams",
                table: "DepartmentHead",
                newName: "UQ__Departme__D0300D45EA024CB1");

            migrationBuilder.RenameIndex(
                name: "UQ__AuditTea__7007AF73218720A0",
                schema: "ams",
                table: "AuditTeam",
                newName: "UQ__AuditTea__7007AF733DCB199C");

            migrationBuilder.RenameColumn(
                name: "OldValue",
                schema: "log",
                table: "AuditLog",
                newName: "Data");

            migrationBuilder.RenameIndex(
                name: "UQ__AuditApp__592EA2A7168E30A8",
                schema: "ams",
                table: "AuditApproval",
                newName: "UQ__AuditApp__592EA2A7366B9DD9");

            migrationBuilder.AddPrimaryKey(
                name: "PK__UserAcco__1788CCACF4537956",
                schema: "auth",
                table: "UserAccount",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__RootCaus__EAE5E40E141DB7EE",
                schema: "ams",
                table: "RootCause",
                column: "RootCauseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8A2B6161FA3AFAF9",
                schema: "ams",
                table: "Role",
                column: "RoleName");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ReportRe__1609F931C53C5848",
                schema: "ams",
                table: "ReportRequest",
                column: "ReportRequestID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Notifica__20CF2E32D3D37E2C",
                schema: "ams",
                table: "Notification",
                column: "NotificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__FindingS__B9A531DF589A3323",
                schema: "ams",
                table: "FindingStatus",
                column: "FindingStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK__FindingS__96F5CD158AB2DB08",
                schema: "ams",
                table: "FindingSeverity",
                column: "Severity");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Finding__19D671C2547241BC",
                schema: "ams",
                table: "Finding",
                column: "FindingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Departme__04A19341CE4BC1D4",
                schema: "ams",
                table: "DepartmentHead",
                column: "DeptHeadID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Departme__0148818EC40042CD",
                schema: "ams",
                table: "Department",
                column: "DeptID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Checklis__F87ADD07BBFA0B57",
                schema: "ams",
                table: "ChecklistTemplate",
                column: "TemplateID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Checklis__727E83EB1BF68FE1",
                schema: "ams",
                table: "ChecklistItem",
                column: "ItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditTea__7093F172CE7CC93A",
                schema: "ams",
                table: "AuditTeam",
                column: "AuditTeamID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditSta__45D7E6365E6E9D5F",
                schema: "ams",
                table: "AuditStatus",
                column: "AuditStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditSco__405679D3A645CC18",
                schema: "ams",
                table: "AuditScopeDepartment",
                column: "AuditScopeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditLog__5E5499A840991FB9",
                schema: "log",
                table: "AuditLog",
                column: "LogID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditDoc__3EF1888DFCF43C9F",
                schema: "ams",
                table: "AuditDocument",
                column: "DocID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditCri__FE6ADB2DA611FA2C",
                schema: "ams",
                table: "AuditCriteria",
                column: "CriteriaID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditChe__E37F18C209B145A9",
                schema: "ams",
                table: "AuditChecklistItem",
                column: "AuditItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__AuditApp__1C6F87158CF157F6",
                schema: "ams",
                table: "AuditApproval",
                column: "AuditApprovalID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Audit_Cr__6E998E0A225AF767",
                schema: "ams",
                table: "Audit_Criteria_Map",
                columns: new[] { "AuditID", "CriteriaID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK__Audit__A17F23B89C9D3A29",
                schema: "ams",
                table: "Audit",
                column: "AuditID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Attachme__F21D4CA2922583A4",
                schema: "ams",
                table: "AttachmentEntityType",
                column: "EntityType");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Attachme__442C64DEEF5BB30F",
                schema: "ams",
                table: "Attachment",
                column: "AttachmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ActionSt__7B3DC58EC9726C81",
                schema: "ams",
                table: "ActionStatus",
                column: "ActionStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Action__FFE3F4B991421ECC",
                schema: "ams",
                table: "Action",
                column: "ActionID");
        }
    }
}
