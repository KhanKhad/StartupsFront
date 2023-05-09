using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private string _authorName;
        private string _contributorsString;

        public int AuthorId { get; set; }
        public int[] Contributors {  get; set; }
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

        public string AuthorName
        {
            get => _authorName;
            set
            {
                _authorName = value;
                OnPropertyChanged();
            }
        }

        public string ContributorsString
        {
            get => _contributorsString;
            set
            {
                _contributorsString = value;
                OnPropertyChanged();
            }
        }

        public INavigation Navigation { get; internal set; }

        public Command JoinToStartupCmd { get; }
        public Command ToChatCmd { get; }
        public Command RefreshCmd { get; }

        
        public StartupViewModel()
        {
            JoinToStartupCmd = new Command(async () => await JoinToStartup());
            ToChatCmd = new Command(async () => await ToChat());
            RefreshCmd = new Command(async () => await Refresh());
            Me = DataStore.MainModel.UserOrNull;
            DataStore.MainModel.UserChanged += UserChanged;
        }

        private async Task Refresh()
        {
            if(IsBusy) return;
            IsBusy = true;
            var startup = await ResponseHelper.GetStartupById(Id, true);
            AuthorId = startup.AuthorForeignKey;
            Contributors = startup.Contributors.ToArray();
            Name = startup.Name;
            Description = startup.Description;
            PictureFileName = startup.StartupPicFileName;
            await SetAuthorAndContributors();
            IsBusy = false;
        }

        public async Task SetAuthorAndContributors()
        {
            var author = await ResponseHelper.GetUserById(AuthorId);
            var contributors = new List<UserModel>();
            foreach (var contributorId in Contributors)
            {
                contributors.Add(await ResponseHelper.GetUserById(contributorId));
            }
            var s = string.Join(", ", contributors.Select(i => i.Name).ToArray());
            ContributorsString = s;
            AuthorName = author.Name;
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
