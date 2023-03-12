using StartupsFront.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace StartupsFront.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}