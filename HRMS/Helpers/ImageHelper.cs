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
            string[] files = Directory.GetFiles(uploadsFolderPath, $"{employeeCode}.*");

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted file: {file}");
                }
                catch (IOException ex)
                {
                    // Log or handle the exception if the file is in use or deletion fails
                    Console.WriteLine($"Failed to delete file: {file}. Error: {ex.Message}");
                }
            }

            return files.Length > 0;
        }
    }
}