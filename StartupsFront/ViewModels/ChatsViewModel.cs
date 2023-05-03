using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ChatsViewModel : BaseViewModel
    {
        private UserModel _user;
        private ChatViewModel _selectedChat;
        private int _delta;
        public INavigation Navigation { get; set; }
        public wObservableCollection<ChatViewModel> Chats { get; set; }

        public Command ChatClick { get; }

        public ChatViewModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged();
            }
        }

        public ChatsViewModel()
        {
            Chats = new wObservableCollection<ChatViewModel>();

            ChatClick = new Command(async() => await OpenChat());
            _user = DataStore.MainModel.UserOrNull;
            DataStore.MainModel.UserChanged += UserChanged;
            _delta = 0;
            _ = Task.Run(CheckDelta);
        }

        public async Task OpenChatWith(int id)
        {
            var chat = new ChatViewModel(id) { Navigation = Navigation };
            await chat.SetUser(id);
            Chats.Insert(0, chat);

            await OpenChat(chat);
        }

        private async Task OpenChat(ChatViewModel chat = null)
        {
            ChatViewModel vm;
            if (chat == null)
                vm = SelectedChat;
            else vm = chat;

            var page = new ChatPage()
            {
                BindingContext = vm
            };

            await Navigation.PushAsync(page);
        }

        public async Task CheckDelta()
        {
            while(true)
            {
                if(_user != null)
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var uri = Requests.GetMessagesDelta(_user.Name);

                            var response = await client.GetAsync(uri);

                            var responseString = await response.Content.ReadAsStringAsync();

                            if (int.TryParse(responseString, out var newDelta))
                            {
                                if (newDelta != _delta)
                                {
                                    await GetAllMessages(_delta);
                                    _delta = newDelta;
                                }
                                SuccessMessage = "Success";
                            }
                            else ErrorMessage += responseString;
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage += ex.Message;
                        }
                    }
                }
                await Task.Delay(1000);
            }
        }

        public async Task GetAllMessages(int delta)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var hash = await CalculateHash(_user.Name, _user.Token);
                    var uri = Requests.GetMessages(_user.Name, hash, delta);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();
                    
                    var messages = ParseMessages(responseString);

                    foreach (var message in messages)
                    {
                        var chat = Chats.FirstOrDefault(i=>i.User.Id == message.SenderForeignKey);
                        if(chat == null)
                        {
                            chat = new ChatViewModel(message.SenderForeignKey) { Navigation = Navigation };
                            await chat.SetUser(message.SenderForeignKey);
                            Chats.Insert(0, chat);
                        }
                        chat.AddMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }

        public List<MessageModel> ParseMessages(string input)
        {
            var answer = JsonSerializer.Deserialize<MessageModel[]>(input);
            return answer.ToList();
        }

        private void UserChanged(UserModel obj)
        {
            _user = obj;
        }


        private const string _hashKey = "It's my message!";
        public static Task<string> CalculateHash(string authorName, string authorToken)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _hashKey));
                var byteResult = mySHA256.ComputeHash(stream);
                return Task.FromResult(Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", ""));
            }
        }
    }
}
