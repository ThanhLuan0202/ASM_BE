using ASM_Repositories.Entities;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.UsersDTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<xxx, yyy>().ReverseMap();
            CreateMap<Department, ViewDepartment>().ReverseMap();
            CreateMap<CreateDepartment, Department>()
                .ForMember(dest => dest.DeptId, opt => opt.Ignore()) // vì ID do DB tự tạo
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now));
            CreateMap<UpdateDepartment, Department>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<UserAccount, ViewUsers>().ReverseMap();

        }
    }
}
