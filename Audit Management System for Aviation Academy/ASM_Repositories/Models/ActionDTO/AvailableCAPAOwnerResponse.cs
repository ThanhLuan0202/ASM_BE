using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.ActionDTO
{
    /// <summary>
    /// Response chứa danh sách CAPAOwner có sẵn
    /// </summary>
    public class AvailableCAPAOwnerResponse
    {
        public List<AvailableCAPAOwnerItem> CAPAOwners { get; set; } = new List<AvailableCAPAOwnerItem>();
    }

    /// <summary>
    /// Thông tin từng CAPAOwner
    /// </summary>
    public class AvailableCAPAOwnerItem
    {
        /// <summary>
        /// User ID của CAPAOwner
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Email của CAPAOwner
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tên đầy đủ của CAPAOwner
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Role của CAPAOwner (thường là "CAPAOwner")
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
    }
}

