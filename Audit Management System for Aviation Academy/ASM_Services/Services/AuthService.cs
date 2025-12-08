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
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IEmailService emailService)
        {
            _authRepository = authRepository;
            _emailService = emailService;
        }
        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            return await _authRepository.LoginAsync(loginRequest);
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var result = await _authRepository.RegisterAsync(request);
            
            // Gửi email thông báo đăng ký thành công với thông tin đăng nhập
            try
            {
                await _emailService.SendRegistrationEmailAsync(
                    toEmail: result.Email,
                    fullName: result.FullName,
                    email: result.Email,
                    password: request.Password, // Password gốc trước khi hash
                    roleName: result.Role
                );
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw để không ảnh hưởng đến quá trình đăng ký
                // Có thể log vào file hoặc logging service
                // _logger.LogError(ex, "Failed to send registration email to {Email}", result.Email);
            }
            
            return result;
        }

        public async Task<BulkRegisterResponse> BulkRegisterAsync(List<RegisterRequestWithRow> requests)
        {
            var result = await _authRepository.BulkRegisterAsync(requests);
            
            // Gửi email cho từng user đăng ký thành công
            foreach (var successItem in result.SuccessItems)
            {
                try
                {
                    // Tìm RegisterRequest tương ứng để lấy password gốc
                    var originalRequest = requests.FirstOrDefault(r => r.RowNumber == successItem.RowNumber);
                    if (originalRequest != null)
                    {
                        await _emailService.SendRegistrationEmailAsync(
                            toEmail: successItem.Email,
                            fullName: successItem.FullName,
                            email: successItem.Email,
                            password: originalRequest.Request.Password, // Password gốc trước khi hash
                            roleName: originalRequest.Request.RoleName
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không throw để không ảnh hưởng đến quá trình đăng ký
                    // Có thể log vào file hoặc logging service
                    // _logger.LogError(ex, "Failed to send registration email to {Email}", successItem.Email);
                }
            }
            
            return result;
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var result = await _authRepository.ResetPasswordAsync(request);
            
            // Gửi email thông báo mật khẩu mới cho user sau khi reset thành công
            if (result != null && !string.IsNullOrWhiteSpace(result.NewPassword))
            {
                try
                {
                    await _emailService.SendPasswordResetEmailAsync(
                        toEmail: result.Email,
                        fullName: result.FullName,
                        newPassword: result.NewPassword
                    );
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không throw để không ảnh hưởng đến quá trình reset password
                    // Password đã được reset thành công, chỉ có phần gửi email bị lỗi
                    // Có thể log vào file hoặc logging service
                    // _logger.LogError(ex, "Failed to send password reset email to {Email}", result.Email);
                }
            }
            
            return result;
        }

        public async Task<AuditorWithScheduleResponse> GetAuditorsWithScheduleAsync()
        {
            return await _authRepository.GetAuditorsWithScheduleAsync();
        }
    }
}
