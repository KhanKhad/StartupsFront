using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private string _name;
        private string _id;
        private string _description;
        private string _pictureFileName;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string PictureFileName
        {
            get => _pictureFileName;
            set
            {
                _pictureFileName = value;
                OnPropertyChanged();
            }
        }

        public INavigation Navigation { get; internal set; }

        public StartupViewModel()
        {

        }
    }
}
