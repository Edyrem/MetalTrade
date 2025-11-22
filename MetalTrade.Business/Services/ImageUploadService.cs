using MetalTrade.Business.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Services
{
    public class ImageUploadService : FileUploadServiceBase, IImageUploadService
    {
        public ImageUploadService(IWebHostEnvironment env, IEnumerable<string> extensions) : base(env, extensions)
        {
            
        }

        protected override string DefaultFolder => "images";
        protected override IEnumerable<string> PermittedExtensions => new List<string>
        {
            ".jpg", ".jpeg", ".png", ".gif"
        };

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            return await UploadFileAsync(file, folder);
        }

        public Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> file, string folder)
        {
            return UploadFilesAsync(file, folder);
        }

        public async Task DeleteImageAsync(string filePath)
        {
            await DeleteFileAsync(filePath);
        }

        public async Task DeleteImagesAsync(IEnumerable<string> filePaths)
        {
            await DeleteFilesAsync(filePaths);
        }        

        bool IsValidImage(IFormFile file, IEnumerable<string> permittedExtensions)
        {
            return IsValidFileType(file, permittedExtensions);
        }

        bool IImageUploadService.IsValidImage(IFormFile file, IEnumerable<string> permittedExtensions)
        {
            return IsValidImage(file, permittedExtensions);
        }
    }
}
