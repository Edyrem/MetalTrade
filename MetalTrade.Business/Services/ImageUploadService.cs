using MetalTrade.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Services
{
    public class ImageUploadService : IImageUploadService
    {
        
        public Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> UploadFilesAsync(IEnumerable<IFormFile> file, string folder)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool IsImageFile(IFormFile file)
        {
            throw new NotImplementedException();
        }

    }
}
