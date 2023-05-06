using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private string _lastMessage;
        private string _myMessage;

        public UserModel User { get; set; }
        public string LastMessage
        {
            get => _lastMessage; 
            set
            {
                _lastMessage = value;
                OnPropertyChanged();
            }
        }
        public string MyMessage
        {
            get => _myMessage; 
            set
            {
                _myMessage = value;
                OnPropertyChanged();
            }
        }

        // список всех полученных сообщений
        public wObservableCollection<MessageModel> Messages { get; }

        // команда отправки сообщений
        public Command SendMessageCommand { get; }
        public INavigation Navigation { get; set; }

        public ChatViewModel(int userId)
        {
            Messages = new wObservableCollection<MessageModel>();

            SendMessageCommand = new Command(async () => await SendMessage());
        }

        public async Task SetUser(int userId)
        {
            var user = await GetUserById(userId);
            User = user;
        }

        private async Task<UserModel> GetUserById(int userId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Requests.GetUserById(userId));

                var userParseResult = await ResponseHelper.GetUserModelFromResponse(response);

                var user = userParseResult.UserModel;

                return user;
            }
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
                await SendMessage(MyMessage);
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
            using (var client = new HttpClient())
            {
                var user = DataStore.MainModel.UserOrNull;
                var hash = await ChatsViewModel.CalculateHash(user.Name, user.Token);
                var messageModel = new MessageJsonModel() { Message = message, Sender = user.Name, Recipient = User.Name, Hash = hash };

                var content = JsonContent.Create(messageModel);

                var response = await client.PostAsync(Requests.SendMessageUri, content);

                var ans = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    SuccessMessage = ans;
                else ErrorMessage = ans;
            }

        }
    }

    public class MessageJsonModel
    {
        [JsonPropertyName(JsonConstants.MessageText)]
        public string Message { get; set; }

        [JsonPropertyName(JsonConstants.MessageSenderName)]
        public string Sender { get; set; }

        [JsonPropertyName(JsonConstants.MessageRecipientName)]
        public string Recipient { get; set; }

        [JsonPropertyName(JsonConstants.MessageHash)]
        public string Hash { get; set; }
    }
}
