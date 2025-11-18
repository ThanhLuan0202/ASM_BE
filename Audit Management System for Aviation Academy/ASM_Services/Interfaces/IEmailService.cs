using ASM_Services.Models.Email;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest request);
    }
}

