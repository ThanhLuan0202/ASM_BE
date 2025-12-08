namespace ASM_Repositories.Models.LoginDTO
{
    /// <summary>
    /// DTO để reset password cho user
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Email của user cần reset password
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Password mới (nếu null hoặc empty thì sẽ tự động generate password mới)
        /// </summary>
        public string? NewPassword { get; set; }
    }
}

