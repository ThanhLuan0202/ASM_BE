using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ASM_Services.Interfaces.AdminInterfaces;

namespace ASM_Services.Services
{
    public class FirebaseUploadService : IFirebaseUploadService
    {
        private readonly IConfiguration _configuration;

        public FirebaseUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderPath = "Attachments")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty");

            var stream = file.OpenReadStream();
            var bucket = _configuration["FireBase:Bucket"];
            
            if (string.IsNullOrEmpty(bucket))
                throw new InvalidOperationException("Firebase Bucket is not configured");

            // Generate unique filename to avoid conflicts
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            
            var task = new FirebaseStorage(bucket)
                .Child(folderPath)
                .Child(fileName)
                .PutAsync(stream);

            var downloadUrl = await task;
            return downloadUrl;
        }
    }
}

