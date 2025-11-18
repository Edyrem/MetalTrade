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
    public abstract class FileUploadServiceBase : IFileUploadService
    {
        protected abstract string DefaultFolder { get; }
        protected virtual IEnumerable<string> PermittedExtensions => new List<string>
        {
            ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx"
        };

        protected readonly IWebHostEnvironment _env;
        

        public FileUploadServiceBase(IWebHostEnvironment env, IEnumerable<string> extensions)
        {
            _env = env;
            if (extensions != null && extensions.Any())
            {
                PermittedExtensions = extensions as List<string> ?? PermittedExtensions;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (IsValidFileType(file, PermittedExtensions))
            {
                try
                {
                    string uploadsFolder = GetUploadPath(folder);
                    EnsureDirectoryExists(uploadsFolder);
                    string uniqueFileName = GenerateUniqueFileName(file.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return $"/{DefaultFolder}/{folder}/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось загрузить файл", ex);
                }
            }
            else
            {
                throw new InvalidOperationException("Неверный тип файла");
            }
        }

        public async Task<string[]> UploadFilesAsync(IEnumerable<IFormFile> files, string folder)
        {
            var uploadedFilePaths = new List<string>();
            var fileList = files?.ToList() ?? new List<IFormFile>();

            if(!fileList.Any())
            {
                return uploadedFilePaths.ToArray();
            }

            try
            {
                foreach (var file in fileList)
                {
                    var filePath = await UploadFileAsync(file, folder);
                    uploadedFilePaths.Add(filePath);
                }
            }
            catch (Exception ex)
            {
                await DeleteFilesAsync(uploadedFilePaths);
                throw new Exception("Не удалось загрузить файлы", ex);
            }

            return uploadedFilePaths.ToArray();
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            try
            {
                string fullPath = Path.Combine(
                    _env.WebRootPath, 
                    filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );
                
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось удалить файл", ex);
            }
        }

        public async Task DeleteFilesAsync(IEnumerable<string> filePaths)
        {
            var pathsList = filePaths?.ToList() ?? new List<string>();
            if (!pathsList.Any())
            {
                return;
            }

            foreach (var filePath in pathsList)
            {
                await DeleteFileAsync(filePath);
            }
        }

        public bool IsValidFileType(IFormFile file, IEnumerable<string> permittedExtension)
        {
            if(file == null || file.Length == 0)
            {
                return false;
            }

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

        protected string GetUploadPath(string folder)
        {
            return Path.Combine(_env.WebRootPath, folder);
        }

        protected void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
