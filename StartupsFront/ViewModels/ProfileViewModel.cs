using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _imageSource;

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

        public ProfileViewModel()
        {
            var datastore = DataStore;
            if (datastore.MainModel.User !=  null)
            {
                _imageSource = Path.Combine(FileNames.ProfilePictureFileDirectory, datastore.MainModel.User.ProfilePictFileName);
            }
            LoginOrRegisterCommand = new Command(async (o) => await LoginOrRegister_Cmd(o));
        }

        private async Task LoginOrRegister_Cmd(object o)
        {
            var vm = new LoginViewModel() { Navigation = Navigation };

            var page = new LoginPage
            {
                BindingContext = vm
            };

            page.Disappearing += Page_Disappearing;

            await Navigation.PushAsync(page);
        }



        private void Page_Disappearing(object sender, EventArgs e)
        {
            var page = (ContentPage)sender;
            page.Disappearing -= Page_Disappearing;

            var dataStore = DataStore;
            if (dataStore.MainModel.User == null)
                return;

            ImageSource = Path.Combine(FileNames.ProfilePictureFileDirectory, dataStore.MainModel.User.ProfilePictFileName);
            Name = dataStore.MainModel.User.Name;
        }
    }
}
