using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public UserModel User { get; set; }
        public string LastMessage { get; set; }
        public string MyMessage { get; set; }

        // список всех полученных сообщений
        public wObservableCollection<MessageModel> Messages { get; }

        // команда отправки сообщений
        public Command SendMessageCommand { get; }

        public ChatViewModel(int userId)
        {
            Messages = new wObservableCollection<MessageModel>();

            User = GetUserById(userId).Result;

            IsBusy = false;         // отправка сообщения не идет

            SendMessageCommand = new Command(async () => await SendMessage());
        }

        private async Task<UserModel> GetUserById(int userId)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(Requests.GetUserById(userId));

            var userParseResult = await ResponseHelper.GetUserModelFromResponse(response);

            var user = userParseResult.UserModel;
            
            var path = Path.Combine(FileNames.UsersPicturesDirectory, userParseResult.UserPictureName);
            File.WriteAllBytes(path, userParseResult.UserPicture);
            user.ProfilePictFileName = path;

            return user;
        }


        public void AddMessage(MessageModel message)
        {
            Messages.Insert(0, message);
            LastMessage = message.Message;
        }

        // Отправка сообщения
        private async Task SendMessage()
        {
            try
            {
                IsBusy = true;
                await SendMessage(string.Empty);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SendMessage(string message)
        {
            await Task.CompletedTask;
        }
    }
}
