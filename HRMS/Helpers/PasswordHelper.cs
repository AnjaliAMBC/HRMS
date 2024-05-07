using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HRMS.Helpers
{
    public class PasswordHelper
    {
        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";

            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(validChars.Length);
                sb.Append(validChars[randomIndex]);
            }
            return sb.ToString();
        }
    }
}