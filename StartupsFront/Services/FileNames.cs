using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StartupsFront.Services
{
    public class FileNames
    {

        public const string ProfilePictureFileName = "profilePic";

        public static string ProfilePictureFileDirectory => Xamarin.Essentials.FileSystem.AppDataDirectory;
    }
}
