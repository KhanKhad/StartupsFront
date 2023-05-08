using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _imageSource;
        private int _startupsDelta;

        private UserModel UserOrNull { get; set; }
        public INavigation Navigation { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        public Command LoginOrRegisterCommand { get; }

        public wObservableCollection<StartupRequestViewModel> StartupRequests { get; set; }
        public StartupRequestViewModel StartupRequest { get; set; }

        public ProfileViewModel()
        {
            StartupRequests = new wObservableCollection<StartupRequestViewModel>();
            var datastore = DataStore;
            datastore.MainModel.UserChanged += UserChanged;

            if (datastore.MainModel.UserOrNull !=  null)
                UserChanged(datastore.MainModel.UserOrNull);

            LoginOrRegisterCommand = new Command(async (o) => await LoginOrRegister_Cmd(o));

            _ = CheckStartupsDelta();
        }

        private async Task CheckStartupsDelta()
        {
            while (true)
            {
                if (UserOrNull != null)
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var uri = Requests.GetStartupsDelta(UserOrNull.Id);

                            var response = await client.GetAsync(uri);

                            var responseString = await response.Content.ReadAsStringAsync();

                            if (int.TryParse(responseString, out var newDelta))
                            {
                                if (newDelta != _startupsDelta)
                                {
                                    await GetAllStartupsRequestes();
                                    _startupsDelta = newDelta;
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
                await Task.Delay(2000);
            }
        }

        private async Task GetAllStartupsRequestes()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var myId = UserOrNull.Id;

                    var hash = await Requests.GetProfileHashAsync(UserOrNull.Name, UserOrNull.Token);

                    var uri = Requests.GetStartupsRequests(UserOrNull.Id);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();

                    var requestes = ParseStartups(responseString);

                    var tasks = new List<Task<List<StartupRequestViewModel>>>();

                    foreach ( var request in requestes)
                    {
                        var task = CreateStartupRequestAsync(request);
                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);

                    var requestesViewModels = new List<StartupRequestViewModel>();

                    foreach ( var task in tasks)
                    {
                        requestesViewModels.AddRange(task.Result);
                    }

                    Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var req in requestesViewModels)
                        {
                            req.ErrorMessageAct += ErrorMessageInRequest;
                            req.SuccessMessageAct += SuccessMessageInRequest;
                            req.NeedToRemoveMe += NeedToRemoveRequest;
                        }
                        StartupRequests.AddRange(requestesViewModels);

                    });
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

        private void NeedToRemoveRequest(StartupRequestViewModel obj)
        {
            StartupRequests.Remove(obj);
        }

        private void SuccessMessageInRequest(string obj)
        {
            SuccessMessage = obj;
        }

        private void ErrorMessageInRequest(string obj)
        {
            ErrorMessage = obj;
        }

        public async Task<List<StartupRequestViewModel>> CreateStartupRequestAsync(StartupJoinRequestJsonModel request)
        {
            var results = new List<StartupRequestViewModel>();
            var startup = await ResponseHelper.GetStartupById(request.StartupId);

            var users = new List<UserModel>();

            foreach (var id in request.UsersWantToJoin)
            {
                var user = await ResponseHelper.GetUserById(id);
                users.Add(user);
            }

            foreach (var user in users)
            {
                var result = new StartupRequestViewModel() { Startup = startup, User = user };
                results.Add(result);
            }
            return results;
        }


        private StartupJoinRequestJsonModel[] ParseStartups(string responseString)
        {
            var answer = JsonSerializer.Deserialize<StartupJoinRequestJsonModel[]>(responseString);
            return answer.ToArray();
        }

        private void UserChanged(UserModel user)
        {
            ImageSource = Path.Combine(FileNames.ProfilePictureDirectory, user.ProfilePictFileName);
            Name = user.Name;
            UserOrNull = user;
        }

        private async Task LoginOrRegister_Cmd(object o)
        {
            var vm = new LoginViewModel() { Navigation = Navigation };

            var page = new LoginPage
            {
                BindingContext = vm
            };

            await Navigation.PushAsync(page);
        }
    }
    public class StartupJoinRequestJsonModel
    {
        [JsonPropertyName(JsonConstants.StartupId)]
        public int StartupId { get; set; }

        [JsonPropertyName(JsonConstants.StartupWantToJoinList)]
        public int[] UsersWantToJoin { get; set; } = Array.Empty<int>();
    }
}
