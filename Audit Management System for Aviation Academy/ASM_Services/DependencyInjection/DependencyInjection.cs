using ASM_Services.Interfaces;
using ASM_Services.Services.AdminServices;
using ASM_Services.Services.AuthServices;
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













            return service;
        }

    }
}
