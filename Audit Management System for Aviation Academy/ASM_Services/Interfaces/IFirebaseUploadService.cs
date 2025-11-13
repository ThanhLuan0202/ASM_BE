using Microsoft.AspNetCore.Http;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IFirebaseUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderPath = "Attachments");
    }
}

