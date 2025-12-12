using ASM_Repositories.Models.AccessGrantDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAccessGrantRepository
    {
        Task<IssueAccessGrantResponse> IssueAsync(IssueAccessGrantRequest request, string qrToken, string qrUrl);
        Task<IEnumerable<ViewAccessGrant>> GetAccessGrantsAsync(Guid? auditId = null, int? deptId = null);
        Task<VerifyQrTokenResponse> VerifyQrTokenAsync(string qrToken, string verifyCode);
    }
}

