using Newtonsoft.Json;
using StartupsFront.DependencyServiceAll;
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
            _imageSource = FileNames.ProfilePictureFilePath;

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

                    var file_bytes = File.ReadAllBytes(_imageSource);

                    MultipartFormDataContent form = new MultipartFormDataContent
                    {
                        { new StringContent(_username), JsonConstants.UserName },
                        { new StringContent(_username + "@mail.ru"), JsonConstants.UserMail },
                        { new StringContent(_password), JsonConstants.UserPassword },

                        { new ByteArrayContent(file_bytes, 0, file_bytes.Length), JsonConstants.UserPicturePropertyName, JsonConstants.UserPictureFileName }
                    };

                    var response = await client.PostAsync(uri, form);

                    var responseString = await response.Content.ReadAsStringAsync();

                    var answerDefinition = new { Result = "", Token = "" };

                    var answer = JsonConvert.DeserializeAnonymousType(responseString, answerDefinition);
                    try
                    {
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

        async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {ImageSource}");
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
                return;

            // save the file into local storage
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(_imageSource))
                await stream.CopyToAsync(newStream);
        }
    }
}
