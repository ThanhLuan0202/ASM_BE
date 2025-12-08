namespace ASM_Repositories.Models.LoginDTO
{
    /// <summary>
    /// Response sau khi reset password thành công
    /// </summary>
    public class ResetPasswordResponse
    {
        /// <summary>
        /// Email của user đã được reset password
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password mới (chỉ trả về nếu NewPassword không được cung cấp trong request)
        /// </summary>
        public string? NewPassword { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}

