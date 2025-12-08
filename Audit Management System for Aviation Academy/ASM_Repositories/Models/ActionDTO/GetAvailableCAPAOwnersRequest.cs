using System;

namespace ASM_Repositories.Models.ActionDTO
{
    /// <summary>
    /// Request để lấy danh sách CAPAOwner có sẵn (không có Action nào chứa date trong khoảng CreatedAt và DueDate)
    /// </summary>
    public class GetAvailableCAPAOwnersRequest
    {
        /// <summary>
        /// Ngày cần kiểm tra (chỉ ngày tháng năm, không cần giờ)
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Department ID để lọc (optional - nếu có thì chỉ lấy CAPAOwner thuộc department này)
        /// </summary>
        public int? DeptId { get; set; }
    }
}

