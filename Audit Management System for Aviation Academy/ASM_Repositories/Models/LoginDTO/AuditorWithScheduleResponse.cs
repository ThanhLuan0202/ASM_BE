using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.LoginDTO
{
    /// <summary>
    /// Response chứa thông tin auditor và trạng thái lịch
    /// </summary>
    public class AuditorWithScheduleResponse
    {
        public List<AuditorWithScheduleItem> Auditors { get; set; } = new List<AuditorWithScheduleItem>();
    }

    /// <summary>
    /// Thông tin từng auditor
    /// </summary>
    public class AuditorWithScheduleItem
    {
        /// <summary>
        /// User ID của auditor
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Email của auditor
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tên đầy đủ của auditor
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Role của auditor (thường là "Auditor")
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Department ID (nếu có)
        /// </summary>
        public int? DeptId { get; set; }

        /// <summary>
        /// Trạng thái tài khoản
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Có lịch hay không (có trong AuditAssignment với status = "Assigned")
        /// </summary>
        public bool HasSchedule { get; set; }

        /// <summary>
        /// Số lượng assignment đang active (status = "Assigned")
        /// </summary>
        public int ActiveAssignmentCount { get; set; }
    }
}

