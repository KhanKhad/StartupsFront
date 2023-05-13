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
        private object _startupsLocker;

        public INavigation Navigation { get; set; }

        public wObservableCollection<StartupViewModel> Startups { get; set; }

        public StartupViewModel LastTappedStartup { get; set; }

        public Command StartupTappedCmd { get; set; }
        public Command RefreshCmd { get; set; }

        public AllStartupsViewModel()
        {
            Startups = new wObservableCollection<StartupViewModel>();
            StartupTappedCmd = new Command(async () => await StartupTappedAsync());
            RefreshCmd = new Command(async () => await DataRefreshAsync());
            _startupsLocker = new object();
            RefreshCmd.Execute(null);
        }

        private async Task<bool> DataRefreshAsync()
        {
            if (IsBusy) return false;

            IsBusy = true;

            Startups.Clear();
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    var uri = Requests.GetStartupsIds(1, 10);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();

                    if (responseString == "[]")
                    {
                        SuccessMessage = "Success";
                        return true;
                    }

                    var ids = responseString.Trim(new char[] {'[', ']' }).Split(',');

                    try
                    {
                        foreach (var id in ids)
                        {
                            await GetStartupByIdAsync(int.Parse(id));
                        }
                        SuccessMessage = "Success";
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = responseString += Environment.NewLine + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

            }

            IsBusy = false;

            return true;
        }


        private async Task GetStartupByIdAsync(int id)
        {
            var startupModel = new StartupViewModel() { Navigation = Navigation };

            var startup = await ResponseHelper.GetStartupByIdAsync(id);
            startupModel.Id = id;
            startupModel.AuthorId = startup.AuthorForeignKey;
            startupModel.Contributors = startup.Contributors.ToArray();
            startupModel.Name = startup.Name;
            startupModel.Description = startup.Description;
            startupModel.PictureFileName = startup.StartupPicFileName;

            lock (_startupsLocker)
            {
                Startups.Add(startupModel);
            }
        }

        private async Task StartupTappedAsync()
        {
            if(IsBusy) return;

            IsBusy = true;

            var vm = LastTappedStartup;

            await vm.SetAuthorAndContributors();

            var page = new StartupPage()
            {
                BindingContext = vm
            };

            IsBusy = false;

            await Navigation.PushAsync(page);
        }
    }
}
