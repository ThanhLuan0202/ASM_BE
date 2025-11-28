using ASM_Repositories.Models.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<BulkRegisterResponse> BulkRegisterAsync(List<RegisterRequestWithRow> requests);
    }
}
