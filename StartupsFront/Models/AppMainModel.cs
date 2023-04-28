using System;
using System.Collections.Generic;
using System.Text;

namespace StartupsFront.Models
{
    public class AppMainModel
    {
        private UserModel _user;

        public UserModel User
        {
            get => _user; 
            set
            {
                _user = value;
                UserChanged?.Invoke(_user);
            }
        }

        public event Action<UserModel> UserChanged;
        public AppMainModel()
        {

        }

    }
}
