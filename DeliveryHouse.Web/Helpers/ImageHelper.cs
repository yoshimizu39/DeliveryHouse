using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public string UploadImage(byte[] pictureArray, string folder)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/images/{folder}/{file}";
        }
    }
}
