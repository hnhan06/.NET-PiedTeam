using Microsoft.AspNetCore.Http;

namespace piedteam_hocmienphi.service.Utils.MediaService;

public interface IService
{
    public Task<string> UploadImageAsync(IFormFile file);
}