using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using System;
using System.IO;
using System.Linq;
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

        INotificationManager notificationManager;
        int notificationNumber = 0;

        public UserModel Сompanion { get; set; }
        public UserModel Me { get; set; }
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
        public wObservableCollection<MessageViewModel> Messages { get; }

        // команда отправки сообщений
        public Command SendMessageCommand { get; }
        public INavigation Navigation { get; set; }

        public ChatViewModel()
        {
            Me = DataStore.MainModel.UserOrNull;
            Messages = new wObservableCollection<MessageViewModel>();

            SendMessageCommand = new Command(async () => await SendMessageAsync());

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
        }

        public async Task SetUserAsync(int userId)
        {
            var user = await ResponseHelper.GetUserByIdAsync(userId);
            Сompanion = user;
        }


        public void AddMessage(MessageModel message)
        {
            if (Messages.FirstOrDefault(i=>i.Id == message.Id) != null) return;
            var owner = string.Empty;

            if (message.RecipientForeignKey == Сompanion.Id)
            {
                owner = Me.Name;
            }
            else
            {
                owner = Сompanion.Name;
                if(message.MessageSended > ShellPageViewModel.AppStarted)
                {
                    notificationNumber++;
                    string title = $"You get new message from {owner}!";
                    string ntmessage = $"{owner}: {message.Message}";
                    notificationManager.SendNotification(title, ntmessage);
                }
            }

            Messages.Add(new MessageViewModel(message, owner));
            LastMessage = $"{owner}: {message.Message}";
        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
            });
        }

        // Отправка сообщения
        private async Task SendMessageAsync()
        {
            if (string.IsNullOrEmpty(MyMessage)) return;
            try
            {
                IsBusy = true;
                await SendMessageAsync(MyMessage);
                MyMessage = string.Empty;
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

        private async Task SendMessageAsync(string message)
        {
            using (var client = new HttpClient())
            {
                var user = DataStore.MainModel.UserOrNull;
                var hash = await Requests.CalculateMessagesHash(user.Name, user.Token);
                var messageModel = new MessageJsonModel() { Message = message, Sender = user.Name, Recipient = Сompanion.Name, Hash = hash };

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
