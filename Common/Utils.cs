using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common
{
    public class Utils
    {
        const string SEPARATOR = "!!!"; // To act as a separator between the file name and the timestamp, while creating the keyname

        public static string GetBucketName(string userId)
        {
            string bucketName = Constants.APPNAME + userId;
            if (bucketName.Contains("@"))
            {
                bucketName = bucketName.Replace("@", "-");
            }

            return bucketName.ToLower();
        }

        public static string GetKeyName(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var time_epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            return fileName + SEPARATOR + time_epoch;
        }

        public static string GetFileNameFromKeyName(string keyName)
        {
            if (!keyName.Contains(SEPARATOR))
            {
                return String.Empty; // Not a valid keyname
            }

            var split = keyName.Split(SEPARATOR);
            if (split == null)
            {
                return String.Empty;
            }

            return split[0];
        }
    }

}
