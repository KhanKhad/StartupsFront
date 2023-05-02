using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ChatsViewModel : BaseViewModel
    {
        private UserModel _user;
        private string _errorMessage;
        private string _successMessage;
        private int _delta;
        public INavigation Navigation { get; set; }
        public wObservableCollection<ChatViewModel> Chats { get; set; }

        public ChatsViewModel()
        {
            Chats = new wObservableCollection<ChatViewModel>();
            _user = DataStore.MainModel.UserOrNull;
            DataStore.MainModel.UserChanged += UserChanged;
            _delta = 0;
            _ = Task.Run(CheckDelta);
        }

        public async Task CheckDelta()
        {
            while(true)
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
                            chat = new ChatViewModel(message.SenderForeignKey);
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
            var answer = JsonConvert.DeserializeObject<List<MessageModel>>(input);
            return answer;
        }

        private void UserChanged(UserModel obj)
        {
            _user = obj;
        }


        private const string _hashKey = "It's my message!";
        private static Task<string> CalculateHash(string authorName, string authorToken)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _hashKey));
                var byteResult = mySHA256.ComputeHash(stream);
                return Task.FromResult(Convert.ToBase64String(byteResult));
            }
        }
    }
}
