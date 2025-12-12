using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using ASM_Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            //service.AddTransient<xxxx, yyyy>();

            service.AddTransient<IAuthService, AuthService>();
            service.AddTransient<IDepartmentService, DepartmentService>();
            service.AddTransient<IFindingService, FindingService>();
            service.AddTransient<IAuditService, AuditService>();
            service.AddTransient<IChecklistTemplateService, ChecklistTemplateService>();
            service.AddTransient<IChecklistItemService, ChecklistItemService>();
            service.AddTransient<IAuditChecklistItemService, AuditChecklistItemService>();
            service.AddTransient<IRootCauseService, RootCauseService>();
            service.AddTransient<IUsersService, UsersService>();
            service.AddTransient<IDepartmentHeadFindingService, DepartmentHeadFindingService>();
            service.AddTransient<IActionService, ActionService>();
            service.AddTransient<IAuditApprovalService, AuditApprovalService>();
            service.AddTransient<IAuditCriterionService, AuditCriterionService>();
            service.AddTransient<IAuditScopeDepartmentService, AuditScopeDepartmentService>();
            service.AddTransient<IAuditTeamService, AuditTeamService>();
            service.AddTransient<IRoleService, RoleService>();
            service.AddTransient<IReportRequestService, ReportRequestService>();
            service.AddTransient<IFindingStatusService, FindingStatusService>();
            service.AddTransient<IFindingSeverityService, FindingSeverityService>();
            service.AddTransient<IActionStatusService, ActionStatusService>();
            service.AddTransient<IAttachmentEntityTypeService, AttachmentEntityTypeService>();
            service.AddTransient<IAuditStatusService, AuditStatusService>();
            service.AddTransient<IDepartmentHeadService, DepartmentHeadService>();
            service.AddTransient<INotificationService, NotificationService>();
            service.AddTransient<IAuditLogService, AuditLogService>();
            service.AddTransient<IAttachmentService, AttachmentService>();
            service.AddTransient<IFirebaseUploadService, FirebaseUploadService>();
            service.AddTransient<IAuditScheduleService, AuditScheduleService>();
            service.AddTransient<IAuditCriteriaMapService, AuditCriteriaMapService>();
            service.AddTransient<IAuditAssignmentService, AuditAssignmentService>();
            service.AddTransient<IPdfGeneratorService, PdfGeneratorService>();
            service.AddTransient<IAuditDocumentService, AuditDocumentService>();
            service.AddTransient<IEmailService, EmailService>();
            service.AddTransient<IAuditChecklistTemplateMapService, AuditChecklistTemplateMapService>();
            service.AddTransient<IAuditPlanAssignmentService, AuditPlanAssignmentService>();
            service.AddTransient<IChecklistItemNoFindingService, ChecklistItemNoFindingService>();
            service.AddTransient<IAccessGrantService, AccessGrantService>();







            return service;
        }

    }
}
