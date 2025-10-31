using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ams");

            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "ActionStatus",
                schema: "ams",
                columns: table => new
                {
                    ActionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ActionSt__7B3DC58EC9726C81", x => x.ActionStatus);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentEntityType",
                schema: "ams",
                columns: table => new
                {
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Attachme__F21D4CA2922583A4", x => x.EntityType);
                });

            migrationBuilder.CreateTable(
                name: "AuditCriteria",
                schema: "ams",
                columns: table => new
                {
                    CriteriaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReferenceCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditCri__FE6ADB2DA611FA2C", x => x.CriteriaID);
                });

            migrationBuilder.CreateTable(
                name: "AuditStatus",
                schema: "ams",
                columns: table => new
                {
                    AuditStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditSta__45D7E6365E6E9D5F", x => x.AuditStatus);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                schema: "ams",
                columns: table => new
                {
                    DeptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__0148818EC40042CD", x => x.DeptID);
                });

            migrationBuilder.CreateTable(
                name: "FindingSeverity",
                schema: "ams",
                columns: table => new
                {
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FindingS__96F5CD158AB2DB08", x => x.Severity);
                });

            migrationBuilder.CreateTable(
                name: "FindingStatus",
                schema: "ams",
                columns: table => new
                {
                    FindingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FindingS__B9A531DF589A3323", x => x.FindingStatus);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "ams",
                columns: table => new
                {
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8A2B6161FA3AFAF9", x => x.RoleName);
                });

            migrationBuilder.CreateTable(
                name: "RootCause",
                schema: "ams",
                columns: table => new
                {
                    RootCauseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RootCaus__EAE5E40E141DB7EE", x => x.RootCauseID);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                schema: "auth",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeptID = table.Column<int>(type: "int", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(64)", maxLength: 64, nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserAcco__1788CCACF4537956", x => x.UserID);
                    table.ForeignKey(
                        name: "FK__UserAccou__DeptI__48CFD27E",
                        column: x => x.DeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                    table.ForeignKey(
                        name: "FK__UserAccou__RoleN__47DBAE45",
                        column: x => x.RoleName,
                        principalSchema: "ams",
                        principalTable: "Role",
                        principalColumn: "RoleName");
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                schema: "ams",
                columns: table => new
                {
                    AttachmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BlobPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    ContentHash = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: true),
                    RetentionUntil = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Attachme__442C64DEEF5BB30F", x => x.AttachmentID);
                    table.ForeignKey(
                        name: "FK__Attachmen__Entit__123EB7A3",
                        column: x => x.EntityType,
                        principalSchema: "ams",
                        principalTable: "AttachmentEntityType",
                        principalColumn: "EntityType");
                    table.ForeignKey(
                        name: "FK__Attachmen__Uploa__1332DBDC",
                        column: x => x.UploadedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "log",
                columns: table => new
                {
                    LogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__5E5499A840991FB9", x => x.LogID);
                    table.ForeignKey(
                        name: "FK__AuditLog__Perfor__2645B050",
                        column: x => x.PerformedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplate",
                schema: "ams",
                columns: table => new
                {
                    TemplateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Checklis__F87ADD07BBFA0B57", x => x.TemplateID);
                    table.ForeignKey(
                        name: "FK__Checklist__Creat__5629CD9C",
                        column: x => x.CreatedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DepartmentHead",
                schema: "ams",
                columns: table => new
                {
                    DeptHeadID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    DeptID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__04A19341CE4BC1D4", x => x.DeptHeadID);
                    table.ForeignKey(
                        name: "FK__Departmen__DeptI__5070F446",
                        column: x => x.DeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                    table.ForeignKey(
                        name: "FK__Departmen__UserI__5165187F",
                        column: x => x.UserID,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "ams",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32D3D37E2C", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__Entit__208CD6FA",
                        column: x => x.EntityType,
                        principalSchema: "ams",
                        principalTable: "AttachmentEntityType",
                        principalColumn: "EntityType");
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__1F98B2C1",
                        column: x => x.UserID,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportRequest",
                schema: "ams",
                columns: table => new
                {
                    ReportRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportRe__1609F931C53C5848", x => x.ReportRequestID);
                    table.ForeignKey(
                        name: "FK__ReportReq__Reque__2B0A656D",
                        column: x => x.RequestedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Audit",
                schema: "ams",
                columns: table => new
                {
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TemplateID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    Objective = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Audit__A17F23B89C9D3A29", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK__Audit__CreatedBy__6383C8BA",
                        column: x => x.CreatedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Audit__Status__628FA481",
                        column: x => x.Status,
                        principalSchema: "ams",
                        principalTable: "AuditStatus",
                        principalColumn: "AuditStatus");
                    table.ForeignKey(
                        name: "FK__Audit__TemplateI__619B8048",
                        column: x => x.TemplateID,
                        principalSchema: "ams",
                        principalTable: "ChecklistTemplate",
                        principalColumn: "TemplateID");
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItem",
                schema: "ams",
                columns: table => new
                {
                    ItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    TemplateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SeverityDefault = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Checklis__727E83EB1BF68FE1", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK__Checklist__Sever__5DCAEF64",
                        column: x => x.SeverityDefault,
                        principalSchema: "ams",
                        principalTable: "FindingSeverity",
                        principalColumn: "Severity");
                    table.ForeignKey(
                        name: "FK__Checklist__Templ__5BE2A6F2",
                        column: x => x.TemplateID,
                        principalSchema: "ams",
                        principalTable: "ChecklistTemplate",
                        principalColumn: "TemplateID");
                });

            migrationBuilder.CreateTable(
                name: "Audit_Criteria_Map",
                schema: "ams",
                columns: table => new
                {
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriteriaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Audit_Cr__6E998E0A225AF767", x => new { x.AuditID, x.CriteriaID });
                    table.ForeignKey(
                        name: "FK__Audit_Cri__Audit__37703C52",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                    table.ForeignKey(
                        name: "FK__Audit_Cri__Crite__3864608B",
                        column: x => x.CriteriaID,
                        principalSchema: "ams",
                        principalTable: "AuditCriteria",
                        principalColumn: "CriteriaID");
                });

            migrationBuilder.CreateTable(
                name: "AuditApproval",
                schema: "ams",
                columns: table => new
                {
                    AuditApprovalID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditApp__1C6F87158CF157F6", x => x.AuditApprovalID);
                    table.ForeignKey(
                        name: "FK__AuditAppr__Appro__71D1E811",
                        column: x => x.ApproverID,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__AuditAppr__Audit__70DDC3D8",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                });

            migrationBuilder.CreateTable(
                name: "AuditChecklistItem",
                schema: "ams",
                columns: table => new
                {
                    AuditItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditChe__E37F18C209B145A9", x => x.AuditItemID);
                    table.ForeignKey(
                        name: "FK__AuditChec__Audit__76969D2E",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                });

            migrationBuilder.CreateTable(
                name: "AuditDocument",
                schema: "ams",
                columns: table => new
                {
                    DocID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BlobPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsFinalVersion = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditDoc__3EF1888DFCF43C9F", x => x.DocID);
                    table.ForeignKey(
                        name: "FK__AuditDocu__Audit__18EBB532",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                    table.ForeignKey(
                        name: "FK__AuditDocu__Uploa__19DFD96B",
                        column: x => x.UploadedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "AuditScopeDepartment",
                schema: "ams",
                columns: table => new
                {
                    AuditScopeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditSco__405679D3A645CC18", x => x.AuditScopeID);
                    table.ForeignKey(
                        name: "FK__AuditScop__Audit__2FCF1A8A",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                    table.ForeignKey(
                        name: "FK__AuditScop__DeptI__30C33EC3",
                        column: x => x.DeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                });

            migrationBuilder.CreateTable(
                name: "AuditTeam",
                schema: "ams",
                columns: table => new
                {
                    AuditTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleInTeam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsLead = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditTea__7093F172CE7CC93A", x => x.AuditTeamID);
                    table.ForeignKey(
                        name: "FK__AuditTeam__Audit__6A30C649",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                    table.ForeignKey(
                        name: "FK__AuditTeam__UserI__6B24EA82",
                        column: x => x.UserID,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Finding",
                schema: "ams",
                columns: table => new
                {
                    FindingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RootCauseID = table.Column<int>(type: "int", nullable: true),
                    DeptID = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExternalAuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Finding__19D671C2547241BC", x => x.FindingID);
                    table.ForeignKey(
                        name: "FK__Finding__AuditID__7C4F7684",
                        column: x => x.AuditID,
                        principalSchema: "ams",
                        principalTable: "Audit",
                        principalColumn: "AuditID");
                    table.ForeignKey(
                        name: "FK__Finding__AuditIt__7D439ABD",
                        column: x => x.AuditItemID,
                        principalSchema: "ams",
                        principalTable: "AuditChecklistItem",
                        principalColumn: "AuditItemID");
                    table.ForeignKey(
                        name: "FK__Finding__Created__01142BA1",
                        column: x => x.CreatedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Finding__DeptID__00200768",
                        column: x => x.DeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                    table.ForeignKey(
                        name: "FK__Finding__Reviewe__04E4BC85",
                        column: x => x.ReviewerID,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Finding__RootCau__7F2BE32F",
                        column: x => x.RootCauseID,
                        principalSchema: "ams",
                        principalTable: "RootCause",
                        principalColumn: "RootCauseID");
                    table.ForeignKey(
                        name: "FK__Finding__Severit__7E37BEF6",
                        column: x => x.Severity,
                        principalSchema: "ams",
                        principalTable: "FindingSeverity",
                        principalColumn: "Severity");
                    table.ForeignKey(
                        name: "FK__Finding__Status__03F0984C",
                        column: x => x.Status,
                        principalSchema: "ams",
                        principalTable: "FindingStatus",
                        principalColumn: "FindingStatus");
                });

            migrationBuilder.CreateTable(
                name: "Action",
                schema: "ams",
                columns: table => new
                {
                    ActionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    FindingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedDeptID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProgressPercent = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)0),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Action__FFE3F4B991421ECC", x => x.ActionID);
                    table.ForeignKey(
                        name: "FK__Action__Assigned__09A971A2",
                        column: x => x.AssignedBy,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Action__Assigned__0A9D95DB",
                        column: x => x.AssignedTo,
                        principalSchema: "auth",
                        principalTable: "UserAccount",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Action__Assigned__0B91BA14",
                        column: x => x.AssignedDeptID,
                        principalSchema: "ams",
                        principalTable: "Department",
                        principalColumn: "DeptID");
                    table.ForeignKey(
                        name: "FK__Action__FindingI__08B54D69",
                        column: x => x.FindingID,
                        principalSchema: "ams",
                        principalTable: "Finding",
                        principalColumn: "FindingID");
                    table.ForeignKey(
                        name: "FK__Action__Status__0C85DE4D",
                        column: x => x.Status,
                        principalSchema: "ams",
                        principalTable: "ActionStatus",
                        principalColumn: "ActionStatus");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_AssignedBy",
                schema: "ams",
                table: "Action",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Action_AssignedDeptID",
                schema: "ams",
                table: "Action",
                column: "AssignedDeptID");

            migrationBuilder.CreateIndex(
                name: "IX_Action_AssignedTo_Status",
                schema: "ams",
                table: "Action",
                columns: new[] { "AssignedTo", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Action_DueDate",
                schema: "ams",
                table: "Action",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Action_FindingID",
                schema: "ams",
                table: "Action",
                column: "FindingID");

            migrationBuilder.CreateIndex(
                name: "IX_Action_Status",
                schema: "ams",
                table: "Action",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_Entity",
                schema: "ams",
                table: "Attachment",
                columns: new[] { "EntityType", "EntityID" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_UploadedBy",
                schema: "ams",
                table: "Attachment",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_CreatedBy",
                schema: "ams",
                table: "Audit",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Status",
                schema: "ams",
                table: "Audit",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_TemplateID",
                schema: "ams",
                table: "Audit",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Criteria_Map_CriteriaID",
                schema: "ams",
                table: "Audit_Criteria_Map",
                column: "CriteriaID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditApproval_ApproverID",
                schema: "ams",
                table: "AuditApproval",
                column: "ApproverID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditApproval_AuditID",
                schema: "ams",
                table: "AuditApproval",
                column: "AuditID");

            migrationBuilder.CreateIndex(
                name: "UQ__AuditApp__592EA2A7366B9DD9",
                schema: "ams",
                table: "AuditApproval",
                columns: new[] { "AuditID", "ApproverID", "ApprovalLevel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audit_AuditID",
                schema: "ams",
                table: "AuditChecklistItem",
                column: "AuditID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditDocument_AuditID",
                schema: "ams",
                table: "AuditDocument",
                column: "AuditID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditDocument_UploadedBy",
                schema: "ams",
                table: "AuditDocument",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_PerformedBy",
                schema: "log",
                table: "AuditLog",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AuditScopeDepartment_AuditID",
                schema: "ams",
                table: "AuditScopeDepartment",
                column: "AuditID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditScopeDepartment_DeptID",
                schema: "ams",
                table: "AuditScopeDepartment",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTeam_AuditID",
                schema: "ams",
                table: "AuditTeam",
                column: "AuditID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTeam_UserID",
                schema: "ams",
                table: "AuditTeam",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__AuditTea__7007AF733DCB199C",
                schema: "ams",
                table: "AuditTeam",
                columns: new[] { "AuditID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Template",
                schema: "ams",
                table: "ChecklistItem",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItem_SeverityDefault",
                schema: "ams",
                table: "ChecklistItem",
                column: "SeverityDefault");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplate_CreatedBy",
                schema: "ams",
                table: "ChecklistTemplate",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentHead_UserID",
                schema: "ams",
                table: "DepartmentHead",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_DeptHead_DeptID",
                schema: "ams",
                table: "DepartmentHead",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "UQ__Departme__D0300D45EA024CB1",
                schema: "ams",
                table: "DepartmentHead",
                columns: new[] { "DeptID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Finding_AuditID_Status",
                schema: "ams",
                table: "Finding",
                columns: new[] { "AuditID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Finding_AuditItemID",
                schema: "ams",
                table: "Finding",
                column: "AuditItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Finding_CreatedBy",
                schema: "ams",
                table: "Finding",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Finding_DeptID_Severity",
                schema: "ams",
                table: "Finding",
                columns: new[] { "DeptID", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_Finding_ReviewerID",
                schema: "ams",
                table: "Finding",
                column: "ReviewerID");

            migrationBuilder.CreateIndex(
                name: "IX_Finding_RootCauseID",
                schema: "ams",
                table: "Finding",
                column: "RootCauseID");

            migrationBuilder.CreateIndex(
                name: "IX_Finding_Severity",
                schema: "ams",
                table: "Finding",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Finding_Status",
                schema: "ams",
                table: "Finding",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_EntityType",
                schema: "ams",
                table: "Notification",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserID",
                schema: "ams",
                table: "Notification",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequest_RequestedBy",
                schema: "ams",
                table: "ReportRequest",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_User_Dept_Role",
                schema: "auth",
                table: "UserAccount",
                columns: new[] { "DeptID", "RoleName" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_RoleName",
                schema: "auth",
                table: "UserAccount",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "UQ__UserAcco__A9D1053420E1AC15",
                schema: "auth",
                table: "UserAccount",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "Attachment",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "Audit_Criteria_Map",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditApproval",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditDocument",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "log");

            migrationBuilder.DropTable(
                name: "AuditScopeDepartment",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditTeam",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "ChecklistItem",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "DepartmentHead",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "ReportRequest",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "Finding",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "ActionStatus",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditCriteria",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AttachmentEntityType",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditChecklistItem",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "RootCause",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "FindingSeverity",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "FindingStatus",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "Audit",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "AuditStatus",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "ChecklistTemplate",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "UserAccount",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "ams");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "ams");
        }
    }
}
