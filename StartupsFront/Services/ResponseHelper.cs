using StartupsFront.Models;
using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<int, UserModel> _loadedUsers = new ConcurrentDictionary<int, UserModel>();
        private static readonly ConcurrentDictionary<int, StartupModel> _loadedStartups = new ConcurrentDictionary<int, StartupModel>();
        private static readonly ConcurrentDictionary<int, bool> _loadingUsers= new ConcurrentDictionary<int, bool>();
        private static readonly ConcurrentDictionary<int, bool> _loadingStartups = new ConcurrentDictionary<int, bool>();
        public static async Task<UserModel> GetUserById(int userId, bool isMainUser = false)
        {
            if (!isMainUser)
            {
                if(!_loadingUsers.TryAdd(userId, false))
                {
                    bool isLoaded = false;

                    while (isLoaded)
                    {
                        await Task.Delay(500);
                        _loadingUsers.TryGetValue(userId, out isLoaded);
                    }
                }
            }

            if (_loadedUsers.TryGetValue(userId, out var userModel))
                return userModel;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Requests.GetUserById(userId));
                userModel = await GetUserModelFromResponse(response, isMainUser);
            }

            _loadedUsers.TryAdd(userId, userModel);

            if (!isMainUser)
            {
                _loadingUsers[userId] = true;
            }

            return userModel;
        }

        public static async Task<StartupModel> GetStartupById(int id)
        {
            if (!_loadingStartups.TryAdd(id, false))
            {
                bool isLoaded = false;

                while (isLoaded)
                {
                    await Task.Delay(500);
                    _loadingStartups.TryGetValue(id, out isLoaded);
                }
            }

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
                                var contributorsIds = await content.ReadAsStringAsync();
                                if(!string.IsNullOrEmpty(contributorsIds))
                                    startupModel.Contributors = contributorsIds.Split(',').Select(i=> int.Parse(i)).ToList();
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

            _loadedStartups.TryAdd(id, startupModel);

            _loadingStartups[id] = true;

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
