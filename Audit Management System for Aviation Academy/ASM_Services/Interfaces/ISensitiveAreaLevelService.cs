using ASM_Repositories.Models.SensitiveAreaLevelDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface ISensitiveAreaLevelService
    {
        Task<List<ViewSensitiveAreaLevel>> GetAllAsync();
        Task<ViewSensitiveAreaLevel?> GetByIdAsync(string level);
        Task<ViewSensitiveAreaLevel> CreateAsync(CreateSensitiveAreaLevel dto, Guid userId);
        Task<ViewSensitiveAreaLevel> UpdateAsync(string level, UpdateSensitiveAreaLevel dto, Guid userId);
        Task<bool> DeleteAsync(string level, Guid userId);
    }
}

