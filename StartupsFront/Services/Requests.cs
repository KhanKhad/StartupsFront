namespace StartupsFront.Services
{
    public static class Requests
    {
        public const string Url = "http://46.183.163.170:8888/";
        public static string CreateUserMultipartUri => $"{Url}Profile/CreateUserFromMultipart";
        public static string AutenticateAndGetUserMultipartUri => $"{Url}Profile/AutenticateAndGetMultipart";
        public static string CreateStartupFromMultiformUri => $"{Url}StartupsManagement/CreateStartupFromMultiform";
        public static string GetStartupsIdsUri => $"{Url}StartupsManagement/GetStartupsIds";
        public static string GetStartupByIdUri => $"{Url}StartupsManagement/GetStartupById";


        public static string Autenticate(string username, string password) => $"{AutenticateAndGetUserMultipartUri}?name={username}&password={password}";
        public static string GetStartupsIds(int pageNumber, int pageSize) => $"{GetStartupsIdsUri}?pageNumber={pageNumber}&pageSize={pageSize}";
        public static string GetStartupById(int id) => $"{GetStartupByIdUri}?id={id}";
    }
}
