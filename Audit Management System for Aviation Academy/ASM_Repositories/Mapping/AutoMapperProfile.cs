using ASM_Repositories.Entities;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.FindingDTO;
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
                .ForMember(dest => dest.DeptId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now));
            CreateMap<UpdateDepartment, Department>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UserAccount, ViewUsers>().ReverseMap();

            // Findingg
            CreateMap<Finding, ViewFinding>().ReverseMap();
            CreateMap<CreateFinding, Finding>()
                .ForMember(dest => dest.FindingId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.AuditId, opt => opt.MapFrom(src => src.AuditId));
            CreateMap<UpdateFinding, Finding>()
                .ForMember(dest => dest.FindingId, opt => opt.Ignore())
                .ForMember(dest => dest.AuditId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

        }
    }
}
