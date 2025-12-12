using ASM_Repositories.Models.AccessGrantDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAccessGrantService
    {
        Task<IssueAccessGrantResponse> IssueAsync(IssueAccessGrantRequest request);
        Task<IEnumerable<ViewAccessGrant>> GetAccessGrantsAsync(Guid? auditId = null, int? deptId = null);
        Task<VerifyQrTokenResponse> VerifyQrTokenAsync(string qrToken, string verifyCode);
        Task<ScanQrTokenResponse> ScanQrTokenAsync(string qrToken, Guid scannerUserId);
        Task<VerifyCodeResponse> VerifyCodeAsync(string qrToken, Guid scannerUserId, string verifyCode);
    }
}

