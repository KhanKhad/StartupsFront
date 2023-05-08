using StartupsFront.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class StartupRequestViewModel : BaseViewModel
    {
        public StartupModel Startup { get; set; }

        public UserModel User { get; set; }
        public Command AcceptCmd { get; }
        public Command RejectCmd { get; }

        public StartupRequestViewModel()
        {

        }



    }
}
