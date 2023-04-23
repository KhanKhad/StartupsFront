using StartupsFront.Models;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private string _errorMessage;

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
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
            /*
            var t = DataStore;
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            t.MainModel.User.Token = Guid.NewGuid().ToString();*/
        }

        private async Task<bool> CheckLogAndPass()
        {
            var dataStore = DataStore;
            var user = new UserModel();
            
            var client = new HttpClient();

            var response = await client.GetAsync(Requests.Autenticate(_username, _password));

            try
            {
                var userMultiform = await response.Content.ReadAsMultipartAsync();

                foreach (var content in userMultiform.Contents)
                {
                    switch (content.Headers.ContentDisposition.Name.ToLower())
                    {
                        case JsonConstants.UserName:
                            user.Name = await content.ReadAsStringAsync();
                            break;
                        case JsonConstants.UserToken:
                            user.Token = await content.ReadAsStringAsync();
                            break;
                        case JsonConstants.UserPicturePropertyName:
                            var fileName = FileNames.ProfilePictureFileName + Path.GetExtension(content.Headers.ContentDisposition.FileName);
                            var path = Path.Combine(FileNames.ProfilePictureFileDirectory, fileName);
                            var bytes = await content.ReadAsByteArrayAsync();
                            File.WriteAllBytes(path, bytes);
                            user.ProfilePictFileName = fileName;
                            break;
                    }
                }
                dataStore.MainModel.User = user;
            }
            catch
            {
                var s = await response.Content.ReadAsStringAsync();
                ErrorMessage = s;
                return false;
            }
            
            return true;
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
