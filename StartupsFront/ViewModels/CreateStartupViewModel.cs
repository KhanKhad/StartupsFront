using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class CreateStartupViewModel : BaseViewModel
    {
        private string _startupName;
        private string _startupDescription;
        private string _imageSource;
        private string _errorMessage;

        public Command PickImageCommand { get; }
        public Command CreateStartupCommand { get; }
        public INavigation Navigation { get; set; }

        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }
        public string StartupName
        {
            get { return _startupName; }
            set
            {
                _startupName = value;
                OnPropertyChanged();
            }
        }
        public string StartupDescription
        {
            get { return _startupDescription; }
            set
            {
                _startupDescription = value;
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

        public CreateStartupViewModel()
        {
            PickImageCommand = new Command(async () => await PickImage());
            CreateStartupCommand = new Command(async () => await CreateStartupAsync());
        }


        private async Task CreateStartupAsync()
        {
            await CreateStartupRequest();
        }

        private async Task<bool> CreateStartupRequest()
        {
            var dataStore = DataStore;

            using (var client = new HttpClient())
            {
                try
                {
                    if(dataStore.MainModel.UserOrNull == null)
                        throw new NullReferenceException(nameof(dataStore.MainModel.UserOrNull));

                    var uri = Requests.CreateStartupFromMultiformUri;

                    var hash = await CalculateHash(dataStore.MainModel.UserOrNull.Name, dataStore.MainModel.UserOrNull.Token);

                    MultipartFormDataContent form = new MultipartFormDataContent
                    {
                        { new StringContent(_startupName), JsonConstants.StartupName },
                        { new StringContent(_startupDescription), JsonConstants.StartupDescription },
                        { new StringContent(dataStore.MainModel.UserOrNull.Name), JsonConstants.StartupAuthorName },
                        { new StringContent(hash), JsonConstants.StartupHash },

                    };

                    var file_bytes = File.ReadAllBytes(_imageSource);

                    if (file_bytes.Length != 0)
                        form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), JsonConstants.StartupPicturePropertyName, Path.GetFileName(_imageSource));

                    var response = await client.PostAsync(uri, form);

                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var answerDefinition = new { Result = ""};

                        var answer = JsonConvert.DeserializeAnonymousType(responseString, answerDefinition);

                        if (answer.Result.Equals("Success", StringComparison.OrdinalIgnoreCase))
                            return true;

                        else ErrorMessage = answer.Result;
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
            ImageSource = Path.Combine(FileNames.StartupsPicturesDirectory, photo.FileName);

            await Task.Run(() => {
                var bytes = File.ReadAllBytes(photo.FullPath);
                File.WriteAllBytes(ImageSource, bytes);
            });
        }

        private const string _hashKey = "It's my startup!";
        private Task<string> CalculateHash(string authorName, string authorToken)
        {
            using (SHA256 mySHA256 = SHA256.Create()) 
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _hashKey));
                var byteResult = mySHA256.ComputeHash(stream);
                return Task.FromResult(Convert.ToBase64String(byteResult));
            }
        }
    }
}
