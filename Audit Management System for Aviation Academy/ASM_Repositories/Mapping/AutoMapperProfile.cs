using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Models.ChecklistItemDTO;
using ASM_Repositories.Models.ChecklistTemplateDTO;
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

            // Audit mappings
            CreateMap<Audit, ViewAudit>().ReverseMap();
            CreateMap<CreateAudit, Audit>()
                .ForMember(dest => dest.AuditId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); 
            CreateMap<UpdateAudit, Audit>()
                .ForMember(dest => dest.AuditId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); 

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
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore()); // Không được update TemplateId

        }
    }
}
