using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class ImageHelper
    {
        public static bool DoesImageExistForEmployee(string employeeCode, string uploadsFolderPath)
        {
            // Directory where the images are stored
            string imagesDirectory = uploadsFolderPath; // Change this to your actual directory

            // Check if any file with the employee code prefix exists in the directory
            string[] files = Directory.GetFiles(imagesDirectory, $"{employeeCode}.*");

            // Delete each file
            foreach (var file in files)
            {
                File.Delete(file);
                Console.WriteLine($"Deleted file: {file}");
            }

            return files.Length > 0;
        }
    }
}