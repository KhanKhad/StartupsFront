namespace StartupsFront.Services
{
    public static class Requests
    {
        public const string Url = "http://46.183.163.170:8888/";
        public static string CreateUserMultipartUri => $"{Url}Profile/CreateUserFromMultipart";
        public static string AutenticateAndGetUserMultipartUri => $"{Url}Profile/AutenticateAndGetMultipart";
        public static string CreateStartupFromMultiformUri => $"{Url}Startups/CreateStartupFromMultiform";
        public static string GetStartupsIdsUri => $"{Url}Startups/GetStartupsIds";
        public static string GetStartupByIdUri => $"{Url}Startups/GetStartupById";
        public static string GetUserByIdUri => $"{Url}Users/GetUserById";
        public static string SendMessageUri => $"{Url}Messages/SendMessage";
        public static string GetMessagesUri => $"{Url}Messages/GetMessages";
        public static string GetMessagesDeltaUri => $"{Url}Messages/GetDelta";


        public static string Autenticate(string username, string password) => $"{AutenticateAndGetUserMultipartUri}?name={username}&password={password}";
        public static string GetStartupsIds(int pageNumber, int pageSize) => $"{GetStartupsIdsUri}?pageNumber={pageNumber}&pageSize={pageSize}";
        public static string GetStartupById(int id) => $"{GetStartupByIdUri}?id={id}";
        public static string GetUserById(int id) => $"{GetUserByIdUri}?id={id}";
        public static string GetMessagesDelta(string name) => $"{GetMessagesDeltaUri}?name={name}";
        public static string GetMessages(string name, string hash, int delta) => $"{GetMessagesUri}?name={name}&hash={hash}&delta={delta}";
    }
}
