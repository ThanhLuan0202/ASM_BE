using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.LoginDTO;
using ASM_Repositories.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IConfiguration _configuration;
        public AuthRepository(IConfiguration config, AuditManagementSystemForAviationAcademyContext DbContext)
        {
            _configuration = config;
            _DbContext = DbContext;
        }
        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _DbContext.UserAccounts
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
                return null;

            // Check if account is blocked
            if (user.Status == "Blocked")
                return null;

            // Verify password
            bool isPasswordCorrect = VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt);

            if (!isPasswordCorrect)
            {
                // Increment failed login count
                user.FailedLoginCount++;

                // Block account after 5 failed attempts
                if (user.FailedLoginCount >= 5)
                {
                    user.Status = "Blocked";
                }

                await _DbContext.SaveChangesAsync();
                return null;
            }

            // Password is correct - reset failed login count and update last login
            user.FailedLoginCount = 0;
            user.LastLogin = DateTime.UtcNow;
            await _DbContext.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                FullName = user.FullName,
                Role = user.RoleName,
                Email = user.Email
            };
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("Email is required");

            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("Password is required");

            if (request.Password.Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters long");

            // Validate fullname
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new InvalidOperationException("Fullname is required");

            // Validate role
            if (string.IsNullOrWhiteSpace(request.RoleName))
                throw new InvalidOperationException("Role is required");

            // Check if email already exists
            var existing = await _DbContext.UserAccounts.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                throw new InvalidOperationException($"Email '{request.Email}' is already registered");
            }

            // Validate Department if provided
            // Note: DeptId đã được tìm và gán từ tên Department trong AuthController
            // Ở đây chỉ validate lại để đảm bảo DeptId tồn tại và active
            if (request.DeptId.HasValue)
            {
                var departmentExists = await _DbContext.Departments
                    .AsNoTracking()
                    .AnyAsync(d => d.DeptId == request.DeptId.Value && d.Status == "Active");
                
                if (!departmentExists)
                {
                    throw new InvalidOperationException($"Department with ID {request.DeptId.Value} not found or is inactive");
                }
            }

            byte[] salt = new byte[32];
            RandomNumberGenerator.Fill(salt); // DB column allows 32 bytes for PasswordSalt
            byte[] hash;
            using (var hmac = new HMACSHA512(salt))
            {
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            }

            // Tạo UserAccount với DeptId đã được tìm từ tên Department
            var user = new UserAccount
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                RoleName = request.RoleName,
                DeptId = request.DeptId, // DeptId đã được gán từ tên Department trong AuthController
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                FailedLoginCount = 0,
                Status = "Active"
            };

            try
            {
                _DbContext.ChangeTracker.Clear();
                await _DbContext.UserAccounts.AddAsync(user);
                await _DbContext.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while registering user: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while registering user: {ex.Message}");
            }

            return new RegisterResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.RoleName,
                DeptId = user.DeptId,
                Status = "Active"
            };
        }
        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(hash);
        }

        private string GenerateJwtToken(UserAccount user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleName),
            new Claim("DeptId", user.DeptId.HasValue ? user.DeptId.ToString() : string.Empty),
            new Claim("FullName", user.FullName)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<BulkRegisterResponse> BulkRegisterAsync(List<RegisterRequestWithRow> requests)
        {
            var response = new BulkRegisterResponse
            {
                TotalRows = requests.Count
            };

            foreach (var requestWithRow in requests)
            {
                try
                {
                    var registerResponse = await RegisterAsync(requestWithRow.Request);
                    response.SuccessCount++;
                    response.SuccessItems.Add(new BulkRegisterItem
                    {
                        RowNumber = requestWithRow.RowNumber,
                        Email = registerResponse.Email,
                        FullName = registerResponse.FullName,
                        UserId = registerResponse.UserId
                    });
                }
                catch (InvalidOperationException ex)
                {
                    // Business logic errors - provide detailed message
                    response.FailureCount++;
                    response.ErrorItems.Add(new BulkRegisterError
                    {
                        RowNumber = requestWithRow.RowNumber,
                        Email = requestWithRow.Request.Email ?? "N/A",
                        ErrorMessage = $"Row {requestWithRow.RowNumber}: {ex.Message}"
                    });
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    // Database errors - provide detailed message
                    response.FailureCount++;
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    response.ErrorItems.Add(new BulkRegisterError
                    {
                        RowNumber = requestWithRow.RowNumber,
                        Email = requestWithRow.Request.Email ?? "N/A",
                        ErrorMessage = $"Row {requestWithRow.RowNumber}: Database error - {innerMessage}"
                    });
                }
                catch (Exception ex)
                {
                    // Unexpected errors - provide full details
                    response.FailureCount++;
                    response.ErrorItems.Add(new BulkRegisterError
                    {
                        RowNumber = requestWithRow.RowNumber,
                        Email = requestWithRow.Request.Email ?? "N/A",
                        ErrorMessage = $"Row {requestWithRow.RowNumber}: Unexpected error - {ex.GetType().Name}: {ex.Message}"
                    });
                }
            }

            return response;
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("Email is required");

            // Tìm user theo email
            var user = await _DbContext.UserAccounts
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                throw new InvalidOperationException($"User with email '{request.Email}' not found");

            // Kiểm tra user có bị blocked không
            if (user.Status == "Blocked")
                throw new InvalidOperationException($"User account with email '{request.Email}' is blocked. Please contact administrator.");

            // Generate password mới nếu không có NewPassword
            string newPassword;
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                // Generate password mới (8-12 ký tự, có chữ hoa, chữ thường, số)
                newPassword = GenerateRandomPassword();
            }
            else
            {
                // Validate password mới
                if (request.NewPassword.Length < 6)
                    throw new InvalidOperationException("New password must be at least 6 characters long");

                newPassword = request.NewPassword;
            }

            // Hash password mới
            byte[] salt = new byte[32];
            RandomNumberGenerator.Fill(salt);
            byte[] hash;
            using (var hmac = new HMACSHA512(salt))
            {
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            }

            // Update password và reset failed login count
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.FailedLoginCount = 0; // Reset failed login count
            user.Status = "Active"; // Đảm bảo status là Active

            try
            {
                _DbContext.UserAccounts.Update(user);
                await _DbContext.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while resetting password: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while resetting password: {ex.Message}");
            }

            // Trả về response
            var response = new ResetPasswordResponse
            {
                Email = user.Email,
                Message = "Password has been reset successfully"
            };

            // Chỉ trả về password mới nếu được generate tự động (không có trong request)
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                response.NewPassword = newPassword;
                response.Message = "Password has been reset successfully. Please use the new password provided.";
            }

            return response;
        }

        /// <summary>
        /// Generate random password (8-12 ký tự, có chữ hoa, chữ thường, số)
        /// </summary>
        private string GenerateRandomPassword()
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string allChars = lowercase + uppercase + numbers;

            var random = new Random();
            int length = random.Next(8, 13); // 8-12 ký tự

            var password = new StringBuilder(length);
            
            // Đảm bảo có ít nhất 1 chữ hoa, 1 chữ thường, 1 số
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);

            // Thêm các ký tự ngẫu nhiên
            for (int i = 3; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle password để không có pattern rõ ràng
            var passwordArray = password.ToString().ToCharArray();
            for (int i = passwordArray.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
            }

            return new string(passwordArray);
        }
    }
}


