using ASM_Repositories.Entities;
using ASM_Repositories.Models.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginResponse?> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<BulkRegisterResponse> BulkRegisterAsync(List<RegisterRequestWithRow> requests);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<AuditorWithScheduleResponse> GetAuditorsWithScheduleAsync();
    }
}
