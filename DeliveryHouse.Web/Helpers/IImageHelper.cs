using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);

        string UploadImage(byte[] pictureArray, string folder);
    }
}
