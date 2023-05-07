using StartupsFront.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StartupsFront.Services
{
    public static class ResponseHelper
    {
        private static readonly Dictionary<int, UserModel> _loadedUsers = new Dictionary<int, UserModel>();
        private static readonly Dictionary<int, StartupModel> _loadedStartups = new Dictionary<int, StartupModel>();

        public static async Task<UserModel> GetUserById(int userId, bool isMainUser = false)
        {
            if(_loadedUsers.TryGetValue(userId, out var userModel))
                return userModel;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Requests.GetUserById(userId));
                userModel = await GetUserModelFromResponse(response, isMainUser);
            }

            lock (_loadedUsers)
            {
                if(!_loadedUsers.ContainsKey(userId))
                    _loadedUsers.Add(userId, userModel);
            }

            return userModel;
        }

        public static async Task<StartupModel> GetStartupById(int id)
        {
            if (_loadedStartups.TryGetValue(id, out var startupModel))
                return startupModel;

            using (var client = new HttpClient())
            {
                startupModel = new StartupModel();

                var uri = Requests.GetStartupById(id);

                var response = await client.GetAsync(uri);

                try
                {
                    var startupMultiform = await response.Content.ReadAsMultipartAsync();

                    foreach (var content in startupMultiform.Contents)
                    {
                        switch (content.Headers.ContentDisposition.Name.ToLower())
                        {
                            case JsonConstants.StartupId:
                                startupModel.Id = int.Parse(await content.ReadAsStringAsync());
                                break;
                            case JsonConstants.StartupAuthorId:
                                startupModel.AuthorForeignKey = int.Parse(await content.ReadAsStringAsync());
                                break;
                            case JsonConstants.StartupName:
                                startupModel.Name = await content.ReadAsStringAsync();
                                break;
                            case JsonConstants.StartupDescription:
                                startupModel.Description = await content.ReadAsStringAsync();
                                break;
                            case JsonConstants.StartupContributorsIds:
                                startupModel.Contributors = (await content.ReadAsStringAsync()).Split(',').Select(i=> int.Parse(i)).ToList();
                                break;
                            case JsonConstants.StartupPicturePropertyName:
                                var fileName = content.Headers.ContentDisposition.FileName;
                                var path = Path.Combine(FileNames.StartupsPicturesDirectory, fileName);
                                if (!File.Exists(path))
                                {
                                    var bytes = await content.ReadAsByteArrayAsync();
                                    File.WriteAllBytes(path, bytes);
                                }
                                startupModel.StartupPicFileName = path;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var s = await response.Content.ReadAsStringAsync();
                    var msg = ex.Message + Environment.NewLine + s;
                    throw new Exception(msg);
                }
            }

            lock(_loadedStartups)
            {
                if (!_loadedStartups.ContainsKey(id))
                    _loadedStartups.Add(id, startupModel);
            }
            

            return startupModel;
        }

        public static async Task<UserModel> GetUserModelFromResponse(HttpResponseMessage responseMessage, bool isMainUser = false)
        {
            var result = new UserModelFromResponse();
            var user = new UserModel();

            var userMultiform = await responseMessage.Content.ReadAsMultipartAsync();

            foreach (var content in userMultiform.Contents)
            {
                switch (content.Headers.ContentDisposition.Name.ToLower())
                {
                    case JsonConstants.UserId:
                        user.Id = int.Parse(await content.ReadAsStringAsync());
                        break;
                    case JsonConstants.UserName:
                        user.Name = await content.ReadAsStringAsync();
                        break;
                    case JsonConstants.UserToken:
                        user.Token = await content.ReadAsStringAsync();
                        break;
                    case JsonConstants.UserPicturePropertyName:
                        result.UserPictureName = content.Headers.ContentDisposition.FileName;
                        var bytes = await content.ReadAsByteArrayAsync();
                        result.UserPicture = bytes;
                        break;
                }
            }

            if (isMainUser)
            {
                var fileName = FileNames.ProfilePictureFileName + Path.GetExtension(result.UserPictureName);
                var path = Path.Combine(FileNames.ProfilePictureDirectory, fileName);
                File.WriteAllBytes(path, result.UserPicture);
                user.ProfilePictFileName = fileName;
            }
            else
            {
                var path = Path.Combine(FileNames.UsersPicturesDirectory, result.UserPictureName);

                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, result.UserPicture);
                }

                user.ProfilePictFileName = path;
            }

            return user;
        }

    }
    public class UserModelFromResponse
    {
        public string UserPictureName { get; set; }
        public byte[] UserPicture { get; set; }
    }
}
