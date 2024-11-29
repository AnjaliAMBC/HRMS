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


        public static string GetDefaultAssetImage(string assetType)
        {
            switch (assetType)
            {
                case "Laptop":
                    return "~/Assets/01_Laptop.png";
                case "Mouse":
                    return "~/Assets/01_Mouse.png";
                case "Headset":
                    return "~/Assets/Headset.png";
                case "Keyboard":
                    return "~/Assets/01_Keyboard.png";
                case "Monitor":
                    return "~/Assets/01_Desktop.png";
                case "CPU":
                    return "~/Assets/01_CPU.png";
                case "Power Cable":
                    return "~/Assets/PowerCable.png";
                case "Charger":
                    return "~/Assets/Charger.png";
                case "UPS":
                    return "~/Assets/UPS.png";
                case "Others":
                default:
                    return "~/Assets/Others.png";
            }
        }
    }
}

