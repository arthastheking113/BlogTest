﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Service
{
    public class BasicImageService : IImageService
    {
        public async Task<byte[]> EncodeFileAsync(IFormFile image)
        {
            if (image == null)
            {
                return null;
            }
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            return stream.ToArray();
        }

        public string RecordContentType(IFormFile image)
        {
            if (image == null)
            {
                return null;
            }
            return image.ContentType;
        }
        public string DecodeFile(byte[] imageData, string contentType)
        {
            if (imageData == null)
            {
                return "http://placehold.it/750x300";
            }
            var imageArray = Convert.ToBase64String(imageData);
            return $"data:{contentType};base64,{imageArray}";
        }
    }
}