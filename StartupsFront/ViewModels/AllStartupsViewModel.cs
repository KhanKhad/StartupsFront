using StartupsFront.Models;
using StartupsFront.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class AllStartupsViewModel
    {
        public INavigation Navigation { get; set; }

        public wObservableCollection<StartupViewModel> Startups { get; set; }

        public StartupViewModel LastTappedStartup { get; set; }

        public Command StartupTappedCmd { get; set; }

        public AllStartupsViewModel()
        {
            Startups = new wObservableCollection<StartupViewModel>();
            GetStartups();
        }
        public void GetStartups()
        {
            StartupTappedCmd = new Command(async() => await StartupTapped());
            var startups = new List<StartupViewModel>();

            startups.Add(new StartupViewModel() { Name = "1", Description = "111"});
            startups.Add(new StartupViewModel() { Name = "2", Description = "222"});
            startups.Add(new StartupViewModel() { Name = "3", Description = "111" });
            startups.Add(new StartupViewModel() { Name = "4", Description = "222" });

            Startups.AddRange(startups);
        }

        private async Task StartupTapped()
        {
            await Task.Delay(10);
        }
    }
}
