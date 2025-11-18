using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Interfaces
{
    public interface IImageUploadService
    {
        Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> file, string folder);
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task DeleteImageAsync(string filePath);
        Task DeleteImagesAsync(IEnumerable<string> filePaths);
        bool IsValidImage(IFormFile file, IEnumerable<string> permittedExtensions);
    }
}
