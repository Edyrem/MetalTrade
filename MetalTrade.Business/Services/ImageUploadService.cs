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

        protected override string DefaultFolder => "Images";
        protected override IEnumerable<string> PermittedExtensions => new List<string>
        {
            ".jpg", ".jpeg", ".png", ".gif"
        };

    }
}
