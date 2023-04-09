﻿using StartupsFront.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StartupsFront.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllStartupsPage : ContentPage
    {
        public AllStartupsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as AllStartupsViewModel)?.OnAppearing();
        }
    }
}