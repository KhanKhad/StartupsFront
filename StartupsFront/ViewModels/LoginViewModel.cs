using StartupsFront.DependencyServiceAll;
using StartupsFront.Models;
using StartupsFront.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private bool _passAlert;
        public string Login
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        public bool Alert
        {
            get { return _passAlert; }
            set
            {
                _passAlert = value;
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

            Alert = !loginResult;

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
            user.Name = "Ahalay";
            dataStore.MainModel.User = user;


            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            return await Task.FromResult(true);

            /*if (stream != null)
            {
                image.Source = ImageSource.FromStream(() => stream);
            }

            
            /*var client = new HttpClient();

            var uri = $"http://127.0.0.1:8080/profile/createuser";

            var response = await client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();

            return !responseString.Contains("unmatch");*/
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
            var vm = new RegisterPageViewModel();
            var page = new RegisterPage()
            {
                BindingContext = vm,
            };
            await Navigation.PushAsync(page);
        }
    }
}
