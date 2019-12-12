using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

namespace N2.Edit.FileSystem
{
    public class FileHelper
    {
        public static string GetCloudReadyFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("FileName is null or empty!");
            }

            fileName = GetCloudReadyFileString(fileName);
            fileName = RemoveIllegalCharacters(fileName, new[] { '.', '-' });

            return fileName;
        }

        public static string GetCloudReadyLocalPath(string localPath)
        {
            var lastIndexOf = localPath.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndexOf > 0)
            {
                var dirName = localPath.Substring(lastIndexOf + 1);
                dirName = GetCloudReadyFileString(dirName);
                dirName = RemoveIllegalCharacters(dirName, new[] { '-' });
                return localPath.Substring(0, lastIndexOf + 1) + dirName;
            }

            return localPath;
        }

        public static string GetCloudReadyFileString(string fileStringInput)
        {
            var fileString = fileStringInput.ToLower();

            fileString = fileString.Replace("å", "a");
            fileString = fileString.Replace("ä", "a");
            fileString = fileString.Replace("ö", "o");

            fileString = fileString.Replace(" ", "-");

            fileString = fileString.Replace("_", "-");  // This is bether to use for web!

            return fileString;
        }

        public static string RemoveIllegalCharacters(string fileStringInput, params char[] allowedCharacters)
        {
            return new string(Array.FindAll(fileStringInput.ToCharArray(), c => char.IsLetterOrDigit(c) || ((IList) allowedCharacters).Contains(c)));
        }
    }
}
