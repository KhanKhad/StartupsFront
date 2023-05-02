using StartupsFront.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StartupsFront.Services
{
    public static class ResponseHelper
    {
        public static async Task<UserModelFromResponse> GetUserModelFromResponse(HttpResponseMessage responseMessage)
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
            result.UserModel = user;
            return result;
        }

    }
    public class UserModelFromResponse
    {
        public UserModel UserModel { get; set; }
        public string UserPictureName { get; set; }
        public byte[] UserPicture { get; set; }
    }
}
