using StartupsFront.Models;
using StartupsFront.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class AllStartupsViewModel
    {
        public INavigation Navigation { get; set; }

        public wObservableCollection<StartupViewModel> Startups { get; set; }

        public AllStartupsViewModel()
        {
            Startups = new wObservableCollection<StartupViewModel>();
            GetStartups();
        }
        public void GetStartups()
        {
            var startups = new List<StartupViewModel>();

            startups.Add(new StartupViewModel() { Name = "1", Description = "111"});
            startups.Add(new StartupViewModel() { Name = "2", Description = "222"});
            startups.Add(new StartupViewModel() { Name = "1", Description = "111" });
            startups.Add(new StartupViewModel() { Name = "2", Description = "222" });

            // Get the path to a file on internal storage
            var backingFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "count.txt");

            // Get the path to a file in the cache directory
            var cacheFile = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "count.txt");

            Startups.AddRange(startups);
        }
    }
}
