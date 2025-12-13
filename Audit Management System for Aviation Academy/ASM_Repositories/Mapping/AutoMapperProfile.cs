using ASM_Repositories.Entities;
using ASM_Repositories.Models.AccessGrantDTO;
using ASM_Repositories.Models.ActionDTO;
using ASM_Repositories.Models.ActionStatusDTO;
using ASM_Repositories.Models.AttachmentDTO;
using ASM_Repositories.Models.AttachmentEntityTypeDTO;
using ASM_Repositories.Models.AuditApprovalDTO;
using ASM_Repositories.Models.AuditLogDTO;
using ASM_Repositories.Models.AuditStatusDTO;
using ASM_Repositories.Models.DepartmentHeadDTO;
using ASM_Repositories.Models.NotificationDTO;
using ASM_Repositories.Models.AuditCriteriaMapDTO;
using ASM_Repositories.Models.AuditCriterionDTO;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.AuditTeamDTO;
using ASM_Repositories.Models.AuditChecklistItemDTO;
using ASM_Repositories.Models.AuditAssignmentDTO;
using ASM_Repositories.Models.ChecklistItemDTO;
using ASM_Repositories.Models.ChecklistTemplateDTO;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.DepartmentSensitiveAreaDTO;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Models.FindingSeverityDTO;
using ASM_Repositories.Models.FindingStatusDTO;
using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Repositories.Models.RoleDTO;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Repositories.Models.SensitiveAreaLevelDTO;
using ASM_Repositories.Models.UsersDTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using ASM_Repositories.Models.AuditDocumentDTO;
using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using ASM_Repositories.Models.ChecklistItemNoFindingDTO;

namespace ASM_Repositories.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<xxx, yyy>().ReverseMap();

            // Department 
            CreateMap<Department, ViewDepartment>().ReverseMap();
            CreateMap<CreateDepartment, Department>()
                .ForMember(dest => dest.DeptId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now));
            CreateMap<UpdateDepartment, Department>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<(int DeptID, int Count), ViewDepartmentCount>()
                .ForMember(dest => dest.DeptName, opt => opt.Ignore())
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count));

            // User 
            CreateMap<UserAccount, ViewUser>().ReverseMap();
            CreateMap<CreateUser, UserAccount>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.FailedLoginCount, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
            CreateMap<UpdateUser, UserAccount>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
            CreateMap<UserAccount, ViewUserShortInfo>().ReverseMap();

            // Finding mappings
            CreateMap<Finding, ViewFinding>().ReverseMap();
            CreateMap<CreateFinding, Finding>()
                .ForMember(dest => dest.FindingId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) 
                .ForMember(dest => dest.AuditId, opt => opt.MapFrom(src => src.AuditId));
            CreateMap<UpdateFinding, Finding>()
                .ForMember(dest => dest.FindingId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
            CreateMap<Finding, ViewFindingDetail>()
                .ForMember(d => d.AuditItem, o => o.MapFrom(s => s.AuditItem))
                .ForMember(d => d.CreatedByUser, o => o.MapFrom(s => s.CreatedByNavigation))
                .ForMember(d => d.ReviewerByUser, o => o.MapFrom(s => s.Reviewer));

            // Audit mappings
            CreateMap<Audit, ViewAudit>().ReverseMap();
            CreateMap<Audit, ViewAuditPlan>()
                .ForMember(dest => dest.Audit, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation))
                .ForMember(dest => dest.ScopeDepartments, opt => opt.MapFrom(src => src.AuditScopeDepartments))
                .ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => src.AuditCriteriaMaps))
                .ForMember(dest => dest.AuditTeams, opt => opt.MapFrom(src => src.AuditTeams))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.AuditSchedules)).ReverseMap();
            CreateMap<CreateAudit, Audit>()
                .ForMember(dest => dest.AuditId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); 
            CreateMap<UpdateAudit, Audit>()
                .ForMember(dest => dest.AuditId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
            CreateMap<Audit, ViewAuditSummary>()
                .ForMember(dest => dest.TotalFindings, opt => opt.Ignore())
                .ForMember(dest => dest.OpenFindings, opt => opt.Ignore())
                .ForMember(dest => dest.ClosedFindings, opt => opt.Ignore())
                .ForMember(dest => dest.OverdueFindings, opt => opt.Ignore())
                .ForMember(dest => dest.SeverityBreakdown, opt => opt.Ignore())
                .ForMember(dest => dest.ByDepartment, opt => opt.Ignore())
                .ForMember(dest => dest.ByRootCause, opt => opt.Ignore());

            // ChecklistTemplate mappings
            CreateMap<ChecklistTemplate, ViewChecklistTemplate>().ReverseMap();
            CreateMap<CreateChecklistTemplate, ChecklistTemplate>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); 
            CreateMap<UpdateChecklistTemplate, ChecklistTemplate>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); 

            // ChecklistItem mappings
            CreateMap<ChecklistItem, ViewChecklistItem>().ReverseMap();
            CreateMap<CreateChecklistItem, ChecklistItem>()
                .ForMember(dest => dest.ItemId, opt => opt.Ignore()); 
            CreateMap<UpdateChecklistItem, ChecklistItem>()
                .ForMember(dest => dest.ItemId, opt => opt.Ignore())
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore()); 

            // Action
            CreateMap<ASM_Repositories.Entities.Action, ViewAction>().ReverseMap();
            CreateMap<CreateAction, ASM_Repositories.Entities.Action>()
                .ForMember(dest => dest.ActionId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.ClosedAt, opt => opt.Ignore()); 
            CreateMap<UpdateAction, ASM_Repositories.Entities.Action>()
                .ForMember(dest => dest.ActionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedBy, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // AuditApproval 
            CreateMap<AuditApproval, ViewAuditApproval>().ReverseMap();
            CreateMap<CreateAuditApproval, AuditApproval>()
                .ForMember(dest => dest.AuditApprovalId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ApprovedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) 
                .ForMember(dest => dest.Approver, opt => opt.Ignore());  
            CreateMap<UpdateAuditApproval, AuditApproval>()
                .ForMember(dest => dest.AuditApprovalId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditId, opt => opt.Ignore())
                .ForMember(dest => dest.ApproverId, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalLevel, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedAt, opt => opt.Ignore());

            // AuditCriterion 
            CreateMap<AuditCriterion, ViewAuditCriterion>().ReverseMap();
            CreateMap<CreateAuditCriterion, AuditCriterion>()
                .ForMember(dest => dest.CriteriaId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));
            CreateMap<UpdateAuditCriterion, AuditCriterion>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // AuditCriteriaMap mappings
            CreateMap<AuditCriteriaMap, ViewAuditCriteriaMap>().ReverseMap();
            CreateMap<CreateAuditCriteriaMap, AuditCriteriaMap>();
            CreateMap<UpdateAuditCriteriaMap, AuditCriteriaMap>();

            // AuditScopeDepartment
            CreateMap<AuditScopeDepartment, ViewAuditScopeDepartment>().ReverseMap();
            CreateMap<CreateAuditScopeDepartment, AuditScopeDepartment>()
                .ForMember(dest => dest.AuditScopeId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));
            CreateMap<UpdateAuditScopeDepartment, AuditScopeDepartment>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // AuditTeam
            CreateMap<AuditTeam, ViewAuditTeam>().ReverseMap();
            CreateMap<CreateAuditTeam, AuditTeam>()
                .ForMember(dest => dest.AuditTeamId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));
            CreateMap<UpdateAuditTeam, AuditTeam>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // RootCause
            CreateMap<RootCause, ViewRootCause>().ReverseMap();
            CreateMap<CreateRootCause, RootCause>()
                .ForMember(dest => dest.RootCauseId, opt => opt.Ignore());
            CreateMap<UpdateRootCause, RootCause>()
                .ForMember(dest => dest.RootCauseId, opt => opt.Ignore());
            CreateMap<(int RootId, int Count), ViewRootCauseCount>()
                .ForMember(dest => dest.RootCause, opt => opt.Ignore())
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count));

            // Role
            CreateMap<Role, ViewRole>().ReverseMap();
            CreateMap<Role, CreateRole>().ReverseMap();
            CreateMap<Role, UpdateRole>().ReverseMap();

            // ReportRequest
            CreateMap<ReportRequest, ViewReportRequest>();
            CreateMap<CreateReportRequest, ReportRequest>()
                .ForMember(dest => dest.ReportRequestId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"))
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore()); 
            CreateMap<UpdateReportRequest, ReportRequest>()
                .ForMember(dest => dest.CompletedAt, opt => opt.Condition(src => src.CompletedAt != null));

            // FindingStatus
            CreateMap<FindingStatus, ViewFindingStatus>().ReverseMap();
            CreateMap<CreateFindingStatus, FindingStatus>();
            CreateMap<UpdateFindingStatus, FindingStatus>();

            // FindingSeverity
            CreateMap<FindingSeverity, ViewFindingSeverity>().ReverseMap();
            CreateMap<CreateFindingSeverity, FindingSeverity>();
            CreateMap<UpdateFindingSeverity, FindingSeverity>();

            // SensitiveAreaLevel
            CreateMap<SensitiveAreaLevel, ViewSensitiveAreaLevel>().ReverseMap();
            CreateMap<CreateSensitiveAreaLevel, SensitiveAreaLevel>();
            CreateMap<UpdateSensitiveAreaLevel, SensitiveAreaLevel>();

            // AuditChecklistItem
            CreateMap<AuditChecklistItem, ViewAuditChecklistItem>().ReverseMap();
            CreateMap<CreateAuditChecklistItem, AuditChecklistItem>()
                .ForMember(dest => dest.AuditItemId, opt => opt.Ignore());
            CreateMap<UpdateAuditChecklistItem, AuditChecklistItem>()
                .ForMember(dest => dest.AuditItemId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditId, opt => opt.Ignore());

            // ActionStatus
            CreateMap<ActionStatus, ViewActionStatus>().ReverseMap();
            CreateMap<CreateActionStatus, ActionStatus>();
            CreateMap<UpdateActionStatus, ActionStatus>();

            // AttachmentEntityType
            CreateMap<AttachmentEntityType, ViewAttachmentEntityType>().ReverseMap();
            CreateMap<CreateAttachmentEntityType, AttachmentEntityType>();
            CreateMap<UpdateAttachmentEntityType, AttachmentEntityType>();

            // AuditStatus
            CreateMap<AuditStatus, ViewAuditStatus>().ReverseMap();
            CreateMap<CreateAuditStatus, AuditStatus>();
            CreateMap<UpdateAuditStatus, AuditStatus>();

            // DepartmentHead
            CreateMap<DepartmentHead, ViewDepartmentHead>().ReverseMap();
            CreateMap<CreateDepartmentHead, DepartmentHead>()
                .ForMember(dest => dest.DeptHeadId, opt => opt.Ignore())
                .ForMember(dest => dest.Dept, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<UpdateDepartmentHead, DepartmentHead>()
                .ForMember(dest => dest.DeptHeadId, opt => opt.Ignore())
                .ForMember(dest => dest.Dept, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Notification
            CreateMap<Notification, ViewNotification>().ReverseMap();
            CreateMap<CreateNotification, Notification>()
                .ForMember(dest => dest.NotificationId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.EntityTypeNavigation, opt => opt.Ignore());
            CreateMap<UpdateNotification, Notification>()
                .ForMember(dest => dest.NotificationId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.EntityTypeNavigation, opt => opt.Ignore());

            // AuditLog
            CreateMap<AuditLog, ViewAuditLog>().ReverseMap();
            CreateMap<CreateAuditLog, AuditLog>()
                .ForMember(dest => dest.LogId, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByNavigation, opt => opt.Ignore());
            CreateMap<UpdateAuditLog, AuditLog>()
                .ForMember(dest => dest.LogId, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedByNavigation, opt => opt.Ignore());

            // Attachment
            CreateMap<Attachment, ViewAttachment>().ReverseMap();
            CreateMap<CreateAttachment, Attachment>()
                .ForMember(dest => dest.AttachmentId, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.Ignore())
                .ForMember(dest => dest.BlobPath, opt => opt.Ignore())
                .ForMember(dest => dest.ContentType, opt => opt.Ignore())
                .ForMember(dest => dest.SizeBytes, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ContentHash, opt => opt.Ignore())
                .ForMember(dest => dest.EntityTypeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedByNavigation, opt => opt.Ignore());
            CreateMap<UpdateAttachment, Attachment>()
                .ForMember(dest => dest.AttachmentId, opt => opt.Ignore())
                .ForMember(dest => dest.EntityType, opt => opt.Ignore())
                .ForMember(dest => dest.EntityId, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.Ignore())
                .ForMember(dest => dest.BlobPath, opt => opt.Ignore())
                .ForMember(dest => dest.ContentType, opt => opt.Ignore())
                .ForMember(dest => dest.SizeBytes, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ContentHash, opt => opt.Ignore())
                .ForMember(dest => dest.EntityTypeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedByNavigation, opt => opt.Ignore());

            // AuditSchedule
            CreateMap<AuditSchedule, ViewAuditSchedule>().ReverseMap();
            CreateMap<CreateAuditSchedule, AuditSchedule>()
                .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Audit, opt => opt.Ignore());
            CreateMap<UpdateAuditSchedule, AuditSchedule>()
                .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Audit, opt => opt.Ignore());

            // AuditAssignment
            CreateMap<AuditAssignment, ViewAuditAssignment>()
                .ForMember(dest => dest.AuditTitle, opt => opt.MapFrom(src => src.Audit != null ? src.Audit.Title : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Dept != null ? src.Dept.Name : null))
                .ForMember(dest => dest.AuditorName, opt => opt.MapFrom(src => src.Auditor != null ? src.Auditor.FullName : null));
            CreateMap<CreateAuditAssignment, AuditAssignment>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Audit, opt => opt.Ignore())
                .ForMember(dest => dest.Dept, opt => opt.Ignore())
                .ForMember(dest => dest.Auditor, opt => opt.Ignore());
            CreateMap<UpdateAuditAssignment, AuditAssignment>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditId, opt => opt.Ignore())
                .ForMember(dest => dest.DeptId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditorId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Audit, opt => opt.Ignore())
                .ForMember(dest => dest.Dept, opt => opt.Ignore())
                .ForMember(dest => dest.Auditor, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // AuditDocument
            CreateMap<AuditDocument, ViewAuditDocument>().ReverseMap();

            // AuditChecklistTemplateMap
            CreateMap<AuditChecklistTemplateMap, ViewAuditChecklistTemplateMap>();
            CreateMap<CreateAuditChecklistTemplateMap, AuditChecklistTemplateMap>()
                .ForMember(d => d.AssignedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateAuditChecklistTemplateMap, AuditChecklistTemplateMap>();

            // AuditPlanAssignment
            CreateMap<AuditPlanAssignment, ViewAuditPlanAssignment>().ReverseMap();
            CreateMap<CreateAuditPlanAssignment, AuditPlanAssignment>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));
            CreateMap<UpdateAuditPlanAssignment, AuditPlanAssignment>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ChecklistItemNoFinding
            CreateMap<ChecklistItemNoFinding, ViewChecklistItemNoFinding>().ReverseMap();
            CreateMap<CreateChecklistItemNoFinding, ChecklistItemNoFinding>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
            CreateMap<UpdateChecklistItemNoFinding, ChecklistItemNoFinding>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // AccessGrant
            CreateMap<AccessGrant, ViewAccessGrant>().ReverseMap();

            // DepartmentSensitiveArea
            CreateMap<DepartmentSensitiveArea, ViewDepartmentSensitiveArea>()
                .ForMember(dest => dest.SensitiveArea, opt => opt.MapFrom(src => src.SensitiveAreas ?? string.Empty))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Dept != null ? src.Dept.Name : null))
                .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.LevelNavigation != null ? src.LevelNavigation.Level : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByNavigation != null ? src.CreatedByNavigation.FullName : null))
                .ReverseMap();
            
            CreateMap<CreateDepartmentSensitiveArea, DepartmentSensitiveArea>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.SensitiveAreas, opt => opt.MapFrom(src => src.SensitiveArea))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<UpdateDepartmentSensitiveArea, DepartmentSensitiveArea>()
                .ForMember(dest => dest.SensitiveAreas, opt => opt.MapFrom(src => src.SensitiveArea))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
