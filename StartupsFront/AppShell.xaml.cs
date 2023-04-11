using StartupsFront.ViewModels;

namespace StartupsFront
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            var dataContext = new ShellPageViewModel(Navigation);
            BindingContext = dataContext;
        }
    }
}
