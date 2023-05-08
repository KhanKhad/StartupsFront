using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;

namespace StartupsFront.Services
{
    public static class Requests
    {
        public const string Url = "http://46.183.163.170:8888/";

        public const string ProfileController = "Profile/";
        public const string StartupsController = "Startups/";
        public const string MessagesController = "Messages/";
        public const string UsersController = "Users/";

        public static string CreateUserMultipartUri => $"{Url}{ProfileController}CreateUserFromMultipart";
        public static string AutenticateAndGetUserMultipartUri => $"{Url}{ProfileController}AutenticateAndGetMultipart";
        public static string TryToJoinToStartupUri => $"{Url}{ProfileController}TryToJoinToStartup";
        public static string AcceptUserToStartupUri => $"{Url}{ProfileController}AcceptUserToStartup";
        public static string RejectUserToStartupUri => $"{Url}{ProfileController}RejectUserToStartup";
        public static string GetStartupsDeltaUri => $"{Url}{ProfileController}GetStartupsDelta";
        public static string GetStartupsRequestsUri => $"{Url}{ProfileController}GetStartupsJoinRequestes";
        public static string CreateStartupFromMultiformUri => $"{Url}{StartupsController}CreateStartupFromMultiform";
        public static string GetStartupsIdsUri => $"{Url}{StartupsController}GetStartupsIds";
        public static string GetMyStartupsIdsUri => $"{Url}{StartupsController}GetMyStartupsIds";
        public static string GetStartupByIdUri => $"{Url}{StartupsController}GetStartupById";
        public static string SendMessageUri => $"{Url}{MessagesController}SendMessage";
        public static string GetMessagesUri => $"{Url}{MessagesController}GetMessages";
        public static string GetMessagesDeltaUri => $"{Url}{MessagesController}GetDelta";
        public static string GetUserByIdUri => $"{Url}{UsersController}GetUserById";


        public static string Autenticate(string username, string password) => $"{AutenticateAndGetUserMultipartUri}?name={username}&password={password}";
        public static string TryToJoinToStartup(int id, string hash, int startupId) => $"{TryToJoinToStartupUri}?id={id}&hash={hash}&startupId={startupId}";
        public static string AcceptUserToStartup(int id, string hash, int startupId, int userid) => $"{AcceptUserToStartupUri}?id={id}&hash={hash}&startupId={startupId}&userid={userid}";
        public static string RejectUserToStartup(int id, string hash, int startupId, int userid) => $"{RejectUserToStartupUri}?id={id}&hash={hash}&startupId={startupId}&userid={userid}";
        public static string GetStartupsDelta(int id) => $"{GetStartupsDeltaUri}?id={id}";
        public static string GetStartupsRequests(int id) => $"{GetStartupsRequestsUri}?id={id}";

        public static string GetStartupsIds(int pageNumber, int pageSize) => $"{GetStartupsIdsUri}?pageNumber={pageNumber}&pageSize={pageSize}";
        public static string GetMyStartupsIds(int id, int pageNumber, int pageSize) => $"{GetMyStartupsIdsUri}?id={id}&pageNumber={pageNumber}&pageSize={pageSize}";
        public static string GetStartupById(int id) => $"{GetStartupByIdUri}?id={id}";
        public static string GetUserById(int id) => $"{GetUserByIdUri}?id={id}";
        public static string GetMessagesDelta(string name) => $"{GetMessagesDeltaUri}?name={name}";
        public static string GetMessages(string name, string hash, int delta) => $"{GetMessagesUri}?name={name}&hash={hash}&delta={delta}";



        private const string _startupsHashKey = "It's my startup!";
        public static Task<string> CalculateStartupsHash(string authorName, string authorToken)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _startupsHashKey));
                var byteResult = mySHA256.ComputeHash(stream);
                return Task.FromResult(Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", ""));
            }
        }
        private const string _messagesHashKey = "It's my message!";
        public static Task<string> CalculateMessagesHash(string authorName, string authorToken)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _messagesHashKey));
                var byteResult = mySHA256.ComputeHash(stream);
                return Task.FromResult(Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", ""));
            }
        }

        private const string _profileHashKey = "My PProfile?";
        public static Task<string> GetProfileHashAsync(string name, string token)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(name + token + _profileHashKey));
                var byteResult = mySHA256.ComputeHash(stream);
                return Task.FromResult(Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", ""));
            }
            
        }
    }
}
