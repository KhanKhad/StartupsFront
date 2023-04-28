using DLToolkit.Forms.Controls;
using StartupsFront.Services;
using Xamarin.Forms;

namespace StartupsFront
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            FlowListView.Init();
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
