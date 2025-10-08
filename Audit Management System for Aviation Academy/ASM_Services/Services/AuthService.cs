using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.LoginDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            return await _authRepository.LoginAsync(loginRequest);
        }
    }
}
