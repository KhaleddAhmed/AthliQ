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
            //1.Get the Located Folder Path
            //string folderPath = $"C:\\Users\\Khale\\Source\\Repos\\RouteC41.G02\\RouteC41.G02.PL\\wwwroot\\Files\\{folderName}";
            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //2.Get File Name And Make it Unique
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            //3.Get File Path
            string filePath = Path.Combine(folderPath, fileName);

            //4.Save file as streams[Data Per Time]
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
