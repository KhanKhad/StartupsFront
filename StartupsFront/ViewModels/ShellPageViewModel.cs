using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ShellPageViewModel: BaseViewModel
    {
        public static ShellPageViewModel Current { get; set; }

        public INavigation Navigation { get; set; }

        public AllStartupsViewModel AllStartupsViewModel { get; set; }
        public FavoriteViewModel FavoriteViewModel { get; set; }
        public CreateStartupViewModel CreateStartupViewModel { get; set; }
        public ChatsViewModel ChatsViewModel { get; set; }
        public ProfileViewModel ProfileViewModel { get; set; }

        public ShellPageViewModel(INavigation navigation)
        {
            Navigation = navigation;

            AllStartupsViewModel = new AllStartupsViewModel() { Navigation = Navigation };
            FavoriteViewModel = new FavoriteViewModel() { Navigation = Navigation };
            CreateStartupViewModel = new CreateStartupViewModel() { Navigation = Navigation };
            ChatsViewModel = new ChatsViewModel() { Navigation = Navigation };
            ProfileViewModel = new ProfileViewModel() { Navigation = Navigation };

            Current = this;
        }
    }
}
