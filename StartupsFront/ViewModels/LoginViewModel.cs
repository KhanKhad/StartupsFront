using StartupsFront.Models;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _password;

        public string Login
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public Command LoginCommand { get; }
        public Command RegisterPageCmd { get; }
        public Command ForgorPassPageCmd { get; }

        public INavigation Navigation { get; set; }
        public LoginViewModel()
        {
            LoginCommand = new Command(async(o) => await Login_Cmd(o));
            RegisterPageCmd = new Command(async (o) => await Register_Cmd(o));
        }

        private async Task Login_Cmd(object obj)
        {
            var loginResult = await CheckLogAndPass();

            if (loginResult)
                await Navigation.PopAsync();
        }

        private async Task<bool> CheckLogAndPass()
        {
            using(var client = new HttpClient())
            {
                var dataStore = DataStore;

                var response = await client.GetAsync(Requests.Autenticate(_username, _password));

                try
                {
                    var user = await ResponseHelper.GetUserModelFromResponse(response, true);

                    dataStore.MainModel.UserOrNull = user;
                }
                catch (Exception ex)
                {
                    var s = await response.Content.ReadAsStringAsync();
                    ErrorMessage = ex.Message + Environment.NewLine + s;
                    return false;
                }

                return true;
            }
        }

        private void ForgorPass_Cmd()
        {
            /*var page = new ForgotPassPage
            {
                //BindingContext = viewModel
            };

            Navigation.PushAsync(page);*/
        }

        private async Task Register_Cmd(object obj)
        {
            var vm = new RegisterPageViewModel()
            {
                Navigation = Navigation,
            };
            var page = new RegisterPage()
            {
                BindingContext = vm,
            };
            await Navigation.PushAsync(page);
        }
    }
}
