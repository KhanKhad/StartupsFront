using StartupsFront.Services;
using StartupsFront.ViewModels;
using StartupsFront.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StartupsFront
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            DependencyService.Register<DataStore>();
            var mainPage = new AppShell();
            MainPage = mainPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
