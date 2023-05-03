using System.IO;

namespace StartupsFront.Services
{
    public class FileNames
    {

        public const string ProfilePictureFileName = "profilePic";

        public static string AppDataDirectory => Xamarin.Essentials.FileSystem.AppDataDirectory;
        public static string CacheDirectory => Xamarin.Essentials.FileSystem.CacheDirectory;

        public static string ProfilePictureDirectory => Path.Combine(AppDataDirectory, "profile");
        public static string StartupsPicturesDirectory => Path.Combine(AppDataDirectory, "startups");
        public static string UsersPicturesDirectory => Path.Combine(AppDataDirectory, "users");


        public static void Initialize()
        {
            if(!Directory.Exists(ProfilePictureDirectory)) 
                Directory.CreateDirectory(ProfilePictureDirectory);
            if(!Directory.Exists(StartupsPicturesDirectory))
                Directory.CreateDirectory(StartupsPicturesDirectory);
            if (!Directory.Exists(UsersPicturesDirectory))
                Directory.CreateDirectory(UsersPicturesDirectory);
        }
    }
}
