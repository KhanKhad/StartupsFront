﻿using StartupsFront.Models;
using StartupsFront.MVVM;
using StartupsFront.Services;
using StartupsFront.Views;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class ContributingStartupsViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }

        private object _startupsLocker;

        private UserModel UserOrNull { get; set; }
        public wObservableCollection<StartupViewModel> Startups { get; set; }

        public StartupViewModel LastTappedStartup { get; set; }

        public Command StartupTappedCmd { get; set; }
        public Command RefreshCmd { get; set; }

        public ContributingStartupsViewModel()
        {
            Startups = new wObservableCollection<StartupViewModel>();
            StartupTappedCmd = new Command(async () => await StartupTappedAsync());
            RefreshCmd = new Command(async () => await DataRefreshAsync());
            _startupsLocker = new object();
            UserOrNull = DataStore.MainModel.UserOrNull;
            DataStore.MainModel.UserChanged += UserChanged;
            RefreshCmd.Execute(null);
        }

        private void UserChanged(UserModel obj)
        {
            UserOrNull = obj;
            RefreshCmd.Execute(null);
        }

        private async Task<bool> DataRefreshAsync()
        {
            Startups.Clear();
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            if(UserOrNull != null)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var uri = Requests.GetMyStartupsIds(UserOrNull.Id, 1, 10);

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
                            var task = GetStartupByIdAsync(int.Parse(id));
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

                }

            }
            else
            {
                SuccessMessage = "NeedToLogin";
            }
            return true;
        }


        private async Task GetStartupByIdAsync(int id)
        {
            var startupModel = new StartupViewModel() { Navigation = Navigation };

            var startup = await ResponseHelper.GetStartupByIdAsync(id);
            startupModel.Id = id;
            startupModel.AuthorId = startup.AuthorForeignKey;
            startupModel.Name = startup.Name;
            startupModel.Contributors = startup.Contributors.ToArray();
            startupModel.Description = startup.Description;
            startupModel.PictureFileName = startup.StartupPicFileName;

            lock (_startupsLocker)
            {
                Startups.Add(startupModel);
            }
        }

        private async Task StartupTappedAsync()
        {
            if (IsBusy) return;

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
