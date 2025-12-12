using ASM_Repositories.Interfaces;
using ASM_Repositories.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection service)
        {
            //service.AddTransient<Ixxx, yyy>();

            service.AddTransient<IAuthRepository, AuthRepository>();
            service.AddTransient<IDepartmentRepository, DepartmentRepository>();
            service.AddTransient<IFindingRepository, FindingRepository>();
            service.AddTransient<IAuditRepository, AuditRepository>();
            service.AddTransient<IChecklistTemplateRepository, ChecklistTemplateRepository>();
            service.AddTransient<IChecklistItemRepository, ChecklistItemRepository>();
            service.AddTransient<IAuditChecklistItemRepository, AuditChecklistItemRepository>();
            service.AddTransient<IRootCauseRepository, RootCauseRepository>();
            service.AddTransient<IUsersRepository, UsersRepository>();
            service.AddTransient<IDepartmentHeadFindingRepository, DepartmentHeadFindingRepository>();
            service.AddTransient<IActionRepository, ActionRepository>();
            service.AddTransient<IAuditApprovalRepository, AuditApprovalRepository>();
            service.AddTransient<IAuditCriterionRepository, AuditCriterionRepository>();
            service.AddTransient<IAuditScopeDepartmentRepository, AuditScopeDepartmentRepository>();
            service.AddTransient<IAuditTeamRepository, AuditTeamRepository>();
            service.AddTransient<IRoleRepository, RoleRepository>();
            service.AddTransient<IReportRequestRepository, ReportRequestRepository>();
            service.AddTransient<IFindingStatusRepository, FindingStatusRepository>();
            service.AddTransient<IFindingSeverityRepository, FindingSeverityRepository>();
            service.AddTransient<IActionStatusRepository, ActionStatusRepository>();
            service.AddTransient<IAttachmentEntityTypeRepository, AttachmentEntityTypeRepository>();
            service.AddTransient<IAuditStatusRepository, AuditStatusRepository>();
            service.AddTransient<IDepartmentHeadRepository, DepartmentHeadRepository>();
            service.AddTransient<INotificationRepository, NotificationRepository>();
            service.AddTransient<IAuditLogRepository, AuditLogRepository>();
            service.AddTransient<IAttachmentRepository, AttachmentRepository>();
            service.AddTransient<IAuditScheduleRepository, AuditScheduleRepository>();
            service.AddTransient<IAuditCriteriaMapRepository, AuditCriteriaMapRepository>();
            service.AddTransient<IAuditDocumentRepository, AuditDocumentRepository>();
            service.AddTransient<IAuditAssignmentRepository, AuditAssignmentRepository>();
            service.AddTransient<IAuditChecklistTemplateMapRepository, AuditChecklistTemplateMapRepository>();
            service.AddTransient<IAuditPlanAssignmentRepository, AuditPlanAssignmentRepository>();
            service.AddTransient<IChecklistItemNoFindingRepository, ChecklistItemNoFindingRepository>();
            service.AddTransient<IAccessGrantRepository, AccessGrantRepository>();
            service.AddTransient<IDepartmentSensitiveAreaRepository, DepartmentSensitiveAreaRepository>();





            return service;
        }
    }
}
