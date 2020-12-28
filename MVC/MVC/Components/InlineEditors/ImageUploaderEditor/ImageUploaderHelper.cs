using System;
using System.Collections.Generic;

using CMS.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Generic.InlineEditors
{
    /// <summary>
    /// Helper methods for image uploader.
    /// </summary>
    public static class ImageUploaderHelper
    {
        private static readonly HashSet<string> allowedExtensions = new HashSet<string>(new[]
        {
            ".bmp",
            ".gif",
            ".ico",
            ".png",
            ".wmf",
            ".jpg",
            ".jpeg",
            ".tiff",
            ".tif"
        }, StringComparer.OrdinalIgnoreCase);


        public static ActionResult ValidateUploadedFile(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                return new BadRequestObjectResult("Cannot upload file without file name.");
            }

            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName)))
            {
                return new UnsupportedMediaTypeResult();
            }

            return null;
        }
    }
}