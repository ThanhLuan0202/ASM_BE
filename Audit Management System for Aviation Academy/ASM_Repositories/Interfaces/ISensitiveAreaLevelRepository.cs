using ASM_Repositories.Models.SensitiveAreaLevelDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface ISensitiveAreaLevelRepository
    {
        Task<List<ViewSensitiveAreaLevel>> GetAllAsync();
        Task<ViewSensitiveAreaLevel?> GetByIdAsync(string level);
        Task<ViewSensitiveAreaLevel> AddAsync(CreateSensitiveAreaLevel dto);
        Task<ViewSensitiveAreaLevel> UpdateAsync(string level, UpdateSensitiveAreaLevel dto);
        Task<bool> DeleteAsync(string level);
    }
}

