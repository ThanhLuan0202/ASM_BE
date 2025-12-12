using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AccessGrantDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AccessGrantService : IAccessGrantService
    {
        private readonly IAccessGrantRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessGrantService(IAccessGrantRepository repo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IssueAccessGrantResponse> IssueAsync(IssueAccessGrantRequest request)
        {
            // Generate QR token (unique token string)
            var qrToken = GenerateQrToken();

            // Generate QR URL
            var baseUrl = GetBaseUrl();
            var qrUrl = $"{baseUrl}/api/AccessGrants/verify/{qrToken}";

            return await _repo.IssueAsync(request, qrToken, qrUrl);
        }

        public Task<IEnumerable<ViewAccessGrant>> GetAccessGrantsAsync(Guid? auditId = null, int? deptId = null)
        {
            return _repo.GetAccessGrantsAsync(auditId, deptId);
        }

        public Task<VerifyQrTokenResponse> VerifyQrTokenAsync(string qrToken, string verifyCode)
        {
            return _repo.VerifyQrTokenAsync(qrToken, verifyCode);
        }

        private string GenerateQrToken()
        {
            // Generate a secure random token
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                var token = Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
                return token.Length > 43 ? token.Substring(0, 43) : token; // Limit to 43 chars for URL safety
            }
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                return $"{request.Scheme}://{request.Host}";
            }

            // Fallback to configuration or default
            var issuer = _configuration["Jwt:Issuer"];
            if (!string.IsNullOrEmpty(issuer))
            {
                return issuer;
            }

            return "https://localhost:7276"; // Default fallback
        }
    }
}

