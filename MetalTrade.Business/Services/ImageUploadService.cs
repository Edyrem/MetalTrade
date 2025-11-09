using MetalTrade.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Services
{
    public class ImageUploadService : IImageUploadService
    {
        
        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> UploadFilesAsync(IEnumerable<IFormFile> file, string folder)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> DeleteFilesAsync(IEnumerable<string> filePaths)
        {
            throw new NotImplementedException();
        }

        public bool IsImageFile(IFormFile file)
        {
            throw new NotImplementedException();
        }

    }
}
