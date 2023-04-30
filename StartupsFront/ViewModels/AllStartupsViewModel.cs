using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class AllStartupsViewModel : BaseViewModel
    {
        private string _errorMessage;
        private object _startupsLocker;
        public INavigation Navigation { get; set; }

        public wObservableCollection<StartupViewModel> Startups { get; set; }

        public StartupViewModel LastTappedStartup { get; set; }

        public Command StartupTappedCmd { get; set; }
        public Command RefreshCmd { get; set; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public AllStartupsViewModel()
        {
            Startups = new wObservableCollection<StartupViewModel>();
            StartupTappedCmd = new Command(async () => await StartupTapped());
            RefreshCmd = new Command(async () => await DataRefresh());
            _startupsLocker = new object();
            RefreshCmd.Execute(null);
        }

        private async Task<bool> DataRefresh()
        {
            ErrorMessage = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    var uri = Requests.GetStartupsIds(1, 10);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();

                    var ids = responseString.TrimStart('[').TrimEnd(']').Split(',');

                    var tasks = new List<Task>();

                    foreach ( var id in ids)
                    {
                        var task = GetStartupById(int.Parse(id));
                        tasks.Add(task);
                    }
                    try
                    {
                        await Task.WhenAll(tasks);
                    }
                    catch(Exception ex)
                    {
                        ErrorMessage = ErrorMessage += Environment.NewLine + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                return false;
            }
        }


        private async Task GetStartupById(int id)
        {
            using (var client = new HttpClient())
            {
                var startupModel = new StartupViewModel() { Navigation = Navigation };

                var uri = Requests.GetStartupById(id);
                
                var response = await client.GetAsync(uri);

                try
                {
                    var startupMultiform = await response.Content.ReadAsMultipartAsync();

                    foreach (var content in startupMultiform.Contents)
                    {
                        switch (content.Headers.ContentDisposition.Name.ToLower())
                        {
                            case JsonConstants.StartupId:
                                startupModel.Id = await content.ReadAsStringAsync();
                                break;
                            case JsonConstants.StartupName:
                                startupModel.Name = await content.ReadAsStringAsync();
                                break;
                            case JsonConstants.StartupDescription:
                                startupModel.Description = await content.ReadAsStringAsync();
                                break;
                            case JsonConstants.StartupPicturePropertyName:
                                var fileName = content.Headers.ContentDisposition.FileName;
                                var path = Path.Combine(FileNames.StartupsPicturesDirectory, fileName);
                                if (!File.Exists(path))
                                {
                                    var bytes = await content.ReadAsByteArrayAsync();
                                    File.WriteAllBytes(path, bytes);
                                }
                                startupModel.PictureFileName = path;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var s = await response.Content.ReadAsStringAsync();
                    var msg = ex.Message + Environment.NewLine + s;
                    throw new Exception(msg);
                }

                lock (_startupsLocker)
                {
                    Startups.Add(startupModel);
                }
            }
        }

        private async Task StartupTapped()
        {
            var vm = LastTappedStartup;

            var page = new StartupPage()
            {
                BindingContext = vm
            };

            await Navigation.PushAsync(page);
        }
    }
}
