namespace ASM_Repositories.Models.LoginDTO
{
    public class RegisterRequestWithRow
    {
        public RegisterRequest Request { get; set; } = null!;
        public int RowNumber { get; set; }
    }
}

