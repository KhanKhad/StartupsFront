namespace StartupsFront.Services
{
    public static class Requests
    {
        public const string Url = "http://46.183.163.170:8888/";
        public static string CreateUserMultipartUri => $"{Url}profile/CreateUserFromMultipart";
        public static string AutenticateAndGetUserMultipartUri => $"{Url}profile/AutenticateAndGetMultipart";

        public static string Autenticate(string username, string password) => $"{AutenticateAndGetUserMultipartUri}?name={username}&password={password}"; 
    }
}
