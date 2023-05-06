using DLToolkit.Forms.Controls;
using StartupsFront.Services;
using System;
using Xamarin.Forms;

namespace StartupsFront
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            FlowListView.Init();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            FileNames.Initialize();
            DependencyService.Register<DataStore>();
            var mainPage = new AppShell();
            MainPage = mainPage;
        }

        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await MainPage.DisplayAlert("Error", e.ExceptionObject.ToString(), "Ok");
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
