using HikeRent.Services.Interfaces;

namespace HikeRent.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly string[] AllowedExt = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxSize = 2 * 1024 * 1024;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveGearImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File gambar tidak valid.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExt.Contains(ext))
                throw new InvalidOperationException("Format gambar harus jpg, jpeg, png, atau webp.");

            if (file.Length > MaxSize)
                throw new InvalidOperationException("Ukuran gambar maksimal 2MB.");

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

            var uploadsFolder = Path.Combine(webRoot, "uploads", "gear-items");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/gear-items/{fileName}";
        }
    }
}
