using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ShellPageViewModel: BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public AllStartupsViewModel AllStartupsViewModel { get; set; }

        public ShellPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            LoginViewModel = new LoginViewModel() { Navigation = Navigation };
            AllStartupsViewModel = new AllStartupsViewModel() { Navigation = Navigation };
        }
    }
}
