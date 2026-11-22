using Microsoft.AspNetCore.Http;

namespace HikeRent.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveGearImageAsync(IFormFile file);
    }
}
