using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AccessGrantDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AccessGrantRepository : IAccessGrantRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AccessGrantRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IssueAccessGrantResponse> IssueAsync(IssueAccessGrantRequest request, string qrToken, string qrUrl)
        {
            var entity = new AccessGrant
            {
                GrantId = Guid.NewGuid(),
                AuditId = request.AuditId,
                AuditorId = request.AuditorId,
                DeptId = request.DeptId,
                ValidFrom = request.ValidFrom,
                ValidTo = request.ValidTo,
                VerifyCode = request.VerifyCode,
                TtlMinutes = request.TtlMinutes,
                QrToken = qrToken,
                QrUrl = qrUrl,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.AccessGrants.Add(entity);
            await _context.SaveChangesAsync();

            return new IssueAccessGrantResponse
            {
                GrantId = entity.GrantId,
                QrToken = entity.QrToken,
                QrUrl = entity.QrUrl,
                VerifyCode = entity.VerifyCode,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo,
                Status = entity.Status
            };
        }

        public async Task<IEnumerable<ViewAccessGrant>> GetAccessGrantsAsync(Guid? auditId = null, int? deptId = null)
        {
            var query = _context.AccessGrants.AsQueryable();

            if (auditId.HasValue)
            {
                query = query.Where(x => x.AuditId == auditId.Value);
            }

            if (deptId.HasValue)
            {
                query = query.Where(x => x.DeptId == deptId.Value);
            }

            var entities = await query
                .Where(x => x.Status == "Active")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAccessGrant>>(entities);
        }

        public async Task<VerifyQrTokenResponse> VerifyQrTokenAsync(string qrToken, string verifyCode)
        {
            var entity = await _context.AccessGrants
                .FirstOrDefaultAsync(x => x.QrToken == qrToken && x.Status == "Active");

            if (entity == null)
            {
                return new VerifyQrTokenResponse
                {
                    IsValid = false,
                    Message = "QR token not found or inactive"
                };
            }

            // Check if token is expired
            var now = DateTime.UtcNow;
            if (now < entity.ValidFrom || now > entity.ValidTo)
            {
                return new VerifyQrTokenResponse
                {
                    IsValid = false,
                    Message = "QR token is expired or not yet valid"
                };
            }

            // Verify code
            if (entity.VerifyCode != verifyCode)
            {
                return new VerifyQrTokenResponse
                {
                    IsValid = false,
                    Message = "Invalid verify code"
                };
            }

            return new VerifyQrTokenResponse
            {
                GrantId = entity.GrantId,
                AuditId = entity.AuditId,
                AuditorId = entity.AuditorId,
                DeptId = entity.DeptId,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo,
                Status = entity.Status,
                IsValid = true,
                Message = "QR token verified successfully"
            };
        }
    }
}

