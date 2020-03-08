using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorkRegister.Models
{
    public class Utils
    {
        public static void EnsureDirectoryExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static string GetTimeString(TimeSpan? time, bool isSecondsRequired = false)
        {
            if(isSecondsRequired)
            {
                return $"{time?.Hours}:{time?.Minutes}";
            }
            else
            {
                return $"{time?.Hours}:{time?.Minutes}:{time?.Seconds}"; ;
            }


        }

    }


}
