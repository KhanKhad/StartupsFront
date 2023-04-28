using StartupsFront.Models;
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
            datastore.MainModel.UserChanged += UserChanged;

            if (datastore.MainModel.User !=  null)
                UserChanged(datastore.MainModel.User);

            LoginOrRegisterCommand = new Command(async (o) => await LoginOrRegister_Cmd(o));

        }

        private void UserChanged(UserModel user)
        {
            ImageSource = Path.Combine(FileNames.ProfilePictureFileDirectory, user.ProfilePictFileName);
            Name = user.Name;
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
}
