using System;
using System.Collections.Generic;
using System.Text;

namespace StartupsFront.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private string _name;
        private string _id;
        private string _description;

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

        public StartupViewModel()
        {

        }
    }
}
