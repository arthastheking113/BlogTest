using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Service
{
    public interface IImageService
    {
        Task<byte[]> EncodeFileAsync(IFormFile image);

        string DecodeFile(byte[] imageData, string contentType);

        string RecordContentType(IFormFile image);
    }
}
