using System;

namespace N2.Edit.FileSystem
{
    public class FileHelper
    {
        public static string GetSafeFileNameToUseAlsoForCloudStorage(string fileNameSelected)
        {
            if (string.IsNullOrEmpty(fileNameSelected))
            {
                throw new ArgumentException("FileName is null or empty!");
            }

            var fileName = fileNameSelected.ToLower();

            fileName = fileName.Replace("å", "a");
            fileName = fileName.Replace("ä", "a");
            fileName = fileName.Replace("ö", "o");

            fileName = fileName.Replace(" ", "-");

            fileName = fileName.Replace("_", "-");  // This is bether to use for web!

            return fileName;
        }
    }
}
