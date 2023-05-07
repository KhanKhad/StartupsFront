using StartupsFront.MVVM;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class FavoriteViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }

        private object _startupsLocker;

        public wObservableCollection<StartupViewModel> Startups { get; set; }

        public StartupViewModel LastTappedStartup { get; set; }

        public Command StartupTappedCmd { get; set; }
        public Command RefreshCmd { get; set; }

        public FavoriteViewModel()
        {
            Startups = new wObservableCollection<StartupViewModel>();
            StartupTappedCmd = new Command(async () => await StartupTapped());
            RefreshCmd = new Command(async () => await DataRefresh());
            _startupsLocker = new object();
            RefreshCmd.Execute(null);
        }

        private async Task<bool> DataRefresh()
        {
            // RefreshCmd.ChangeCanExecute();
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

                    var ids = responseString.Trim(new char[] { '[', ']' }).Split(',');

                    var tasks = new List<Task>();

                    foreach (var id in ids)
                    {
                        var task = GetStartupById(int.Parse(id));
                        tasks.Add(task);
                    }
                    try
                    {
                        await Task.WhenAll(tasks);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ErrorMessage += Environment.NewLine + ex.Message;
                    }
                    SuccessMessage = "Success";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                return true;
            }
        }


        private async Task GetStartupById(int id)
        {
            var startupModel = new StartupViewModel() { Navigation = Navigation };

            var startup = await ResponseHelper.GetStartupById(id);
            startupModel.Id = id;
            startupModel.AuthorId = startup.AuthorForeignKey;
            startupModel.Name = startup.Name;
            startupModel.Description = startup.Description;
            startupModel.PictureFileName = startup.StartupPicFileName;

            lock (_startupsLocker)
            {
                Startups.Add(startupModel);
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
