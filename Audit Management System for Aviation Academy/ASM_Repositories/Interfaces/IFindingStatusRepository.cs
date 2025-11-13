using ASM_Repositories.Entities;
using ASM_Repositories.Models.FindingStatusDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IFindingStatusRepository
    {
        Task<List<ViewFindingStatus>> GetAllAsync();
        Task<ViewFindingStatus> GetByIdAsync(string status);
        Task<ViewFindingStatus> AddAsync(CreateFindingStatus dto);
        Task<bool> UpdateAsync(string status, UpdateFindingStatus dto);
        Task<bool> DeleteAsync(string status);
    }
}
