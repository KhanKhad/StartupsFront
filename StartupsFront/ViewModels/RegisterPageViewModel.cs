using StartupsFront.DependencyServiceAll;
using StartupsFront.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class RegisterPageViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public Command SignUpCommand { get; }
        public Command PickImageCommand { get; }

        private string _username;
        private string _password;
        private string _password2;
        private string _imageSource;
        private string _errorMessage;
        public string Login
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string Password2
        {
            get { return _password2; }
            set
            {
                _password2 = value;
                OnPropertyChanged();
            }
        }

        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public RegisterPageViewModel()
        {
            // Get the path to a file on internal storage
            _imageSource = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "profilePic.jpg");

            SignUpCommand = new Command(async () => await SignUpCmd());
            PickImageCommand = new Command(async () => await PickImage());
        }

        private async Task SignUpCmd()
        {
            var regRes = await SignUp();
            
            if (regRes)
                await Navigation.PopAsync();
        }

        private async Task<bool> SignUp()
        {
            using (var client = new HttpClient())
            {
                var uri = AppMainModel.Url + "profile/download";

                var file_bytes = File.ReadAllBytes(_imageSource);

                MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new StringContent(_username), "username" },
                    { new StringContent("Ahalaymail"), "email" },
                    { new StringContent(_password), "password" },

                    { new ByteArrayContent(file_bytes, 0, file_bytes.Length), "profile_pic", "profile.png" }
                };

                var response = await client.PostAsync(uri, form);

                var responseString = await response.Content.ReadAsStringAsync();

                var res = responseString.Contains("success");

                if (!res)
                    ErrorMessage = responseString;

                return res;
            }
        }
        private async Task PickImage()
        {
            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            var bytes = await ReadFully(stream);
            await Task.Run(()=> File.WriteAllBytes(_imageSource, bytes));
            OnPropertyChanged(nameof(ImageSource));
        }

        public static async Task<byte[]> ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
