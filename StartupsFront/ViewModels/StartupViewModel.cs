using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private string _name;
        private int _id;
        private string _description;
        private string _pictureFileName;
        private UserModel _author;
        private UserModel[] _contributors;

        public int AuthorId { get; set; }
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string PictureFileName
        {
            get => _pictureFileName;
            set
            {
                _pictureFileName = value;
                OnPropertyChanged();
            }
        }

        public UserModel Me { get; set; }
        public UserModel Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }

        public INavigation Navigation { get; internal set; }

        public Command JoinToStartupCmd { get; }
        public Command ToChatCmd { get; }

        public StartupViewModel()
        {
            JoinToStartupCmd = new Command(async () => await JoinToStartup());
            ToChatCmd = new Command(async () => await ToChat());
            Me = DataStore.MainModel.UserOrNull;
            DataStore.MainModel.UserChanged += UserChanged;
        }

        private void UserChanged(UserModel obj)
        {
            Me = obj;
        }

        private async Task ToChat()
        {
            await ShellPageViewModel.Current.ChatsViewModel.OpenChatWith(AuthorId);
        }

        private async Task JoinToStartup()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    var hash = await Requests.GetProfileHashAsync(Me.Name, Me.Token);

                    var uri = Requests.TryToJoinToStartup(Me.Id, hash, _id);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var answerDefinition = new { Result = "" };

                        var answer = JsonConvert.DeserializeAnonymousType(responseString, answerDefinition);

                        if (answer.Result.Equals("RequestSended", StringComparison.OrdinalIgnoreCase))
                            SuccessMessage = answer.Result;

                        else ErrorMessage = answer.Result;
                    }
                    catch
                    {
                        ErrorMessage = responseString;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }
    }
}
