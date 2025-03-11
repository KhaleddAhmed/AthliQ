using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AthliQ.Service.Helpers
{
    public class DocumentSettings
    {
        public static async Task<string> UploadFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            // Get the Located Folder Path
            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName
            );

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Generate file name based on the content to avoid duplication
            string fileName = file.FileName;
            string filePath = Path.Combine(folderPath, fileName);

            // Check if the file already exists
            if (File.Exists(filePath))
            {
                return fileName; // Return existing file name without uploading
            }

            // Save file as stream
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return fileName;
        }

        public static void Delete(string fileName, string folderName)
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName,
                fileName
            );

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        public static bool CheckNotExist(string fileName, string folderName)
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName,
                fileName
            );

            if (System.IO.File.Exists(filePath))
                return false;
            return true;
        }
    }
}
