using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Shared.Extensions
{
    public static class IFormFileExtensions
    {
        public static async Task<string> WriteTempAsync(this IFormFile IFormFileValue, string TempDirectory)
        {
            // Prepare and copy temp file for processing
            string safeName = SafeFileName(IFormFileValue.FileName);
            string tempPath = Path.Combine(TempDirectory, safeName);

            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await IFormFileValue.CopyToAsync(stream);
            }

            return tempPath;
        }

        public static string GetExtension(this IFormFile IFormFileValue, bool IncludePeriod = true)
        {
            // Get the file extension from this IFormFile file content
            string fileExtension = Path.GetExtension(IFormFileValue.FileName).ToLower();

            return IncludePeriod ? fileExtension : fileExtension[1..];
        }

        public static string SafeFileName(string FileName)
        {
            // Create a safe name for file storing
            var uniqueSafeName = $"{Path.GetFileNameWithoutExtension(FileName).GetSafeName()}_{Guid.NewGuid().GetString(0, 8)}";

            return String.Concat($"{uniqueSafeName}{Path.GetExtension(FileName).ToLower()}");
        }
    }
}
