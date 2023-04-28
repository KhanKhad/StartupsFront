using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StartupsFront.Services
{
    public class FileNames
    {

        public const string ProfilePictureFileName = "profilePic";

        public static string AppDataDirectory => Xamarin.Essentials.FileSystem.AppDataDirectory;
        public static string CacheDirectory => Xamarin.Essentials.FileSystem.CacheDirectory;

        public static string ProfilePictureFileDirectory => AppDataDirectory;
    }
}
