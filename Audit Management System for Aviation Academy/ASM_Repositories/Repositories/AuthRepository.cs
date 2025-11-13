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
         .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
                return null;

            if (!VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
                return null;

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
            var existing = await _DbContext.UserAccounts.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            byte[] salt = new byte[32];
            RandomNumberGenerator.Fill(salt); // DB column allows 32 bytes for PasswordSalt
            byte[] hash;
            using (var hmac = new HMACSHA512(salt))
            {
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            }

            var user = new UserAccount
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                RoleName = request.RoleName,
                DeptId = request.DeptId,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                FailedLoginCount = 0
            };

            _DbContext.ChangeTracker.Clear();
            await _DbContext.UserAccounts.AddAsync(user);
            await _DbContext.SaveChangesAsync();

            return new RegisterResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.RoleName,
                DeptId = user.DeptId
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

        
    }
}


