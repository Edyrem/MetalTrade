namespace MetalTrade.Web.Services.Advertisement
{
    public class AdvertisementPhotoSaveService
    {
        private readonly IWebHostEnvironment _env;
        public AdvertisementPhotoSaveService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<List<string>> SavePhotosAsync(IFormFile[] photos)
        {
            var saved_paths = new List<string>();

            string upload_folder = Path.Combine(_env.WebRootPath, "images", "advertisement");
            if (!Directory.Exists(upload_folder))
                Directory.CreateDirectory(upload_folder);

            foreach (var photo in photos)
            {
                var file_name = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var file_path = Path.Combine(upload_folder, file_name);

                using (var stream = new FileStream(file_path, FileMode.Create))
                    await photo.CopyToAsync(stream);

                saved_paths.Add($"/images/advertisement/{file_name}");
            }
            return saved_paths;
        }
    }
}
