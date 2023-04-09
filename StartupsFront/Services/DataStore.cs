using StartupsFront.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartupsFront.Services
{
    public class DataStore
    {
        public AppMainModel MainModel { get; set; }
        public DataStore()
        {
            MainModel = new AppMainModel();
        }
    }
}