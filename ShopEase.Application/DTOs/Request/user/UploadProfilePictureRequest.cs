using Microsoft.AspNetCore.Http;

namespace ShopEase.Application.DTOs.Request.user
{
    public class UploadProfilePictureRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
