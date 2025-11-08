using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Interfaces
{
    public interface IFileUploadService
    {
        Task<string[]> UploadFilesAsync(IEnumerable<IFormFile> file, string folder);
        Task<string> UploadFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string filePath);
    }
}
