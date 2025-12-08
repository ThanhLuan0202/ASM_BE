using ASM_Repositories.Models.FindingSeverityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IFindingSeverityService
    {
        Task<List<ViewFindingSeverity>> GetAllAsync();
        Task<ViewFindingSeverity?> GetByIdAsync(string severity);
        Task<ViewFindingSeverity> CreateAsync(CreateFindingSeverity dto, Guid userId);
        Task<ViewFindingSeverity> UpdateAsync(string severity, UpdateFindingSeverity dto, Guid userId);
        Task<bool> DeleteAsync(string severity, Guid userId);
    }
}