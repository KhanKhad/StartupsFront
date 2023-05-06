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

                if(value != null)
                {
                    ChatClick.Execute(null);
                    SelectedChat = null;
                }  
            }
        }

        public ChatsViewModel()
        {
            Chats = new wObservableCollection<ChatViewModel>();

            ChatClick = new Command(async() => await OpenChat());
            _user = DataStore.MainModel.UserOrNull;
            DataStore.MainModel.UserChanged += UserChanged;
            _delta = 0;
            _ = CheckDelta();
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
                                Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    SuccessMessage = "Success";
                                });
                            }
                            else
                            {
                                ErrorMessage += responseString;
                            }
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                            {
                                ErrorMessage += ex.Message;
                            });
                        }
                    }
                }
                await Task.Delay(10000);
            }
        }

        public async Task GetAllMessages(int delta)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var myId = _user.Id;

                    var hash = await CalculateHash(_user.Name, _user.Token);

                    var uri = Requests.GetMessages(_user.Name, hash, delta);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();
                    
                    var messages = ParseMessages(responseString);

                    var messagesToMe = messages.Where(i => i.RecipientForeignKey == myId).ToArray();

                    var messagesFromMe = messages.Where(i => i.SenderForeignKey == myId).ToArray();

                    var chats = new Dictionary<int, ChatModel>();

                    foreach ( var message in messagesToMe)
                    {
                        if (chats.TryGetValue(message.SenderForeignKey, out ChatModel chatModel))
                            chatModel.MessagesToMe.Add(message);
                        else 
                        {
                            var chat = new ChatModel();
                            chat.MessagesToMe.Add(message);
                            chats.Add(message.SenderForeignKey, chat);
                        }
                    }

                    foreach (var message in messagesFromMe)
                    {
                        if (chats.TryGetValue(message.RecipientForeignKey, out ChatModel chatModel))
                            chatModel.MessagesFromMe.Add(message);
                        else
                        {
                            var chat = new ChatModel();
                            chat.MessagesFromMe.Add(message);
                            chats.Add(message.RecipientForeignKey, chat);
                        }
                    }

                    foreach ( var chatKey in chats.Keys)
                    {
                        await CreateChat(chatKey, chats[chatKey]);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }

        public async Task CreateChat(int chatCompanionId, ChatModel chatModel)
        {
            try
            {

                var chat = Chats.FirstOrDefault(i => i.Сompanion.Id == chatCompanionId);

                var chatNotExist = chat == null;

                if (chatNotExist)
                {
                    chat = new ChatViewModel(chatCompanionId) { Navigation = Navigation };
                    await chat.SetUser(chatCompanionId);
                }

                var messagesInChat = chatModel.GetAllMessagesSortedByMyDelta();

                
                Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    foreach (var message in messagesInChat)
                    {
                        chat.AddMessage(message);
                    }

                    if(chatNotExist)
                        Chats.Add(chat);
                });
            }
            catch(Exception ex)
            {

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
            _delta = 0;
            Chats.Clear();
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

    public class ChatModel {
        public List <MessageModel> MessagesToMe { get; set;}
        public List <MessageModel> MessagesFromMe { get; set;}

        public ChatModel() 
        {
            MessagesToMe = new List<MessageModel>();
            MessagesFromMe = new List<MessageModel>();
        }

        public List<MessageModel> GetAllMessagesSortedByMyDelta()
        {
            var result = new List<MessageModel>();

            var messagesToMe = MessagesToMe.OrderBy(i => i.RecipientDelta).ToList();

            var messagesFromMe = MessagesFromMe.OrderBy(i => i.SenderDelta).ToList();

            while (messagesToMe.Count > 0 || messagesFromMe.Count > 0)
            {
                if (messagesToMe.Count > 0 && messagesFromMe.Count > 0)
                {
                    if (messagesToMe[0].RecipientDelta < messagesFromMe[0].SenderDelta)
                    {
                        result.Add(messagesToMe[0]);
                        messagesToMe.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(messagesFromMe[0]);
                        messagesFromMe.RemoveAt(0);
                    }
                }
                else
                {
                    if (messagesToMe.Count > 0)
                    {
                        result.Add(messagesToMe[0]);
                        messagesToMe.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(messagesFromMe[0]);
                        messagesFromMe.RemoveAt(0);
                    }
                }
            }
            return result;
        }
    }
}
