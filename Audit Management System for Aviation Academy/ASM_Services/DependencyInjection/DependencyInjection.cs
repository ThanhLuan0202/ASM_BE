using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using ASM_Services.Interfaces.AuthInterfaces.AuthServices;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using ASM_Services.Services.AdminServices;
using ASM_Services.Services.AuthServices;
using ASM_Services.Services.SQAStaffServices;
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
            service.AddTransient<IDepartmentServices, DepartmentService>();
            service.AddTransient<IFindingService, FindingService>();
            service.AddTransient<IAuditService, AuditService>();
            service.AddTransient<IUsersService, UsersService>();












            return service;
        }

    }
}
