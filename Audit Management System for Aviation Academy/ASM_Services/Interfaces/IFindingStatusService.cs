using ASM_Repositories.Models.FindingStatusDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IFindingStatusService
    {
        Task<List<ViewFindingStatus>> GetAllAsync();
        Task<ViewFindingStatus> GetByIdAsync(string status);
        Task<ViewFindingStatus> CreateAsync(CreateFindingStatus dto);
        Task<bool> UpdateAsync(string status, UpdateFindingStatus dto);
        Task<bool> DeleteAsync(string status);
    }
}
