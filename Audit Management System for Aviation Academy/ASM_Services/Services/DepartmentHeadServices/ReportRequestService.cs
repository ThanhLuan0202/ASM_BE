using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.DepartmentHeadServices
{
    public class ReportRequestService : IReportRequestService
    {
        private readonly IReportRequestRepository _repo;

        public ReportRequestService(IReportRequestRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewReportRequest>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewReportRequest?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewReportRequest> CreateAsync(CreateReportRequest dto) => _repo.CreateAsync(dto);
        public Task<ViewReportRequest?> UpdateAsync(Guid id, UpdateReportRequest dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> SoftDeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
    }
}
