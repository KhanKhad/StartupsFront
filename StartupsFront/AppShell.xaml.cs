using StartupsFront.ViewModels;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

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
