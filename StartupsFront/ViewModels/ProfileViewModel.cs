using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;

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

        public Command LoginOrRegisterCommand { get; }

        public ProfileViewModel()
        {
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

            Name = dataStore.MainModel.User.Name;
        }
    }
}
