using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        public string UserName
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
            foreach (var file in new DirectoryInfo(FileNames.ProfilePictureFileDirectory).GetFiles())
            {
                if (file.Name.StartsWith(FileNames.ProfilePictureFileName))
                {
                    _imageSource = Path.Combine(FileNames.ProfilePictureFileDirectory, FileNames.ProfilePictureFileName + file.Extension);
                    break;
                }
            }
            
            SignUpCommand = new Command(async () => await SignUpCmd());
            PickImageCommand = new Command(async () => await PickImage());
        }

        private async Task SignUpCmd()
        {
            ErrorMessage = string.Empty;

            var regRes = await SignUp();
            
            if (regRes)
                await Navigation.PopAsync();
        }

        private async Task<bool> SignUp()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var uri = Requests.CreateUserMultipartUri;

                    MultipartFormDataContent form = new MultipartFormDataContent
                    {
                        { new StringContent(_username), JsonConstants.UserName },
                        { new StringContent(_username + "@mail.ru"), JsonConstants.UserMail },
                        { new StringContent(_password), JsonConstants.UserPassword },

                    };

                    var file_bytes = File.ReadAllBytes(_imageSource);

                    if (file_bytes.Length != 0)
                        form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), JsonConstants.UserPicturePropertyName, _imageSource);

                    var response = await client.PostAsync(uri, form);

                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var answerDefinition = new { Result = "", Token = "" };

                        var answer = JsonConvert.DeserializeAnonymousType(responseString, answerDefinition);

                        if (answer.Result.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        {
                            var user = new UserModel()
                            {
                                Name = _username,
                                Password = _password,
                                Token = answer.Token,
                            };
                            return true;
                        }
                    }
                    catch
                    {
                        ErrorMessage = responseString;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                return false;
            }
        }
        private async Task PickImage()
        {
            await TakePhotoAsync();
            OnPropertyChanged(nameof(ImageSource));
        }

        private async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {ImageSource}");
            }
            catch (FeatureNotSupportedException)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        private async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
                return;
            ImageSource = Path.Combine(FileNames.ProfilePictureFileDirectory, FileNames.ProfilePictureFileName + Path.GetExtension(photo.FileName));
            
            await Task.Run(() =>{
                var bytes = File.ReadAllBytes(photo.FullPath);
                File.WriteAllBytes(ImageSource, bytes);
            });
        }
    }
}
