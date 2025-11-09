using MetalTrade.Business.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Services
{
    public abstract class FileUploadService : IFileUploadService
    {
        protected readonly List<string> _permittedExtensions = new List<string>
        {
            ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx"
        };

        protected readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env, IEnumerable<string> extensions)
        {
            _env = env;
            if (extensions != null && extensions.Any())
            {
                _permittedExtensions = extensions as List<string> ?? _permittedExtensions;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if(IsValidFileType(file, _permittedExtensions))
            {
                try
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, folder);
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return "/" + folder + "/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("File upload failed.", ex);
                }
            else
            {
                throw new InvalidOperationException("Invalid file type.");
            }
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

        public bool IsValidFileType(IFormFile file, IEnumerable<string> permittedExtension)
        {
            var extention = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extention) || !permittedExtension.Contains(extention))
            {
                return false;
            }
            return permittedExtension.Any(x => x == extention);
        }

        protected virtual string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{extension}";
        }
    }
}
