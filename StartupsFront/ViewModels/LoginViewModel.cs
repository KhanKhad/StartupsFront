using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public INavigation Navigation { get; set; }
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private void OnLoginClicked(object obj)
        {
            var t = DataStore;
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            t.MainModel.User.Token = Guid.NewGuid().ToString();
        }
    }
}
