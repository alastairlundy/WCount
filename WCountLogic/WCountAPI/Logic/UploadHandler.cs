using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace WCountAPI.Logic
{
    public class UploadHandler
        
    {
        internal static long SizeLimit = 5 * 1024 * 1024;

        public static string UploadFile(IFormFile file)
        {
            //extension
            var extension = Path.GetExtension(file.FileName);

            string[] validExtensions = [".txt", ".md", ".rtf", ".ini", ".docx", ".doc", ".odt"];

            if (validExtensions.Contains(extension) == false)
            {
                throw new ArgumentException($"Extension is not valid. Valid extensions are: ({string.Join(",", validExtensions)})");
            }

            //file size

            long sizeBytes = file.Length;

            if(sizeBytes > SizeLimit)
            {
                throw new ArgumentException($"Maximum Allowed Size is {SizeLimit / 1024}KB");
            }

            //name change (for security reasons)


            string newFileName = Guid.NewGuid().ToString() + extension;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            using FileStream stream = new FileStream(Path.Combine(path, newFileName), FileMode.Create);

            file.CopyTo(stream);

            return newFileName;
        }
    }
}
