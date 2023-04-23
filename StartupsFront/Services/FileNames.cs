using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StartupsFront.Services
{
    public class FileNames
    {
        private const string ProfilePictureFileName = "profilePic.jpg";

        public static string ProfilePictureFilePath => Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, ProfilePictureFileName);
    }
}
