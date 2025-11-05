using ASM_Repositories.AdminRepositories.Repositories.AdminRepositories;
using ASM_Repositories.AuthRepositories.Repositories.AuthRepositories;
using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Interfaces.AdminInterfaces.AdminRepositories;
using ASM_Repositories.Interfaces.AuthInterfaces.AuthRepositories;
using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Repositories.AdminRepositories;
using ASM_Repositories.Repositories.DepartmentHeadRepositories;
using ASM_Repositories.Repositories.SQAStaffRepositories;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
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
            service.AddTransient<IRootCauseRepository, RootCauseRepository>();
            service.AddTransient<IUsersRepository, UsersRepository>();
            service.AddTransient<IDepartmentHeadFindingRepository, DepartmentHeadFindingRepository>();
            service.AddTransient<IActionRepository, ActionRepository>();
            service.AddTransient<IAuditApprovalRepository, AuditApprovalRepository>();
            service.AddTransient<IAuditCriterionRepository, AuditCriterionRepository>();
            service.AddTransient<IAuditScopeDepartmentRepository, AuditScopeDepartmentRepository>();
            service.AddTransient<IAuditTeamRepository, AuditTeamRepository>();
            service.AddTransient<IRoleRepository, RoleRepository>();







            return service;
        }
    }
}
