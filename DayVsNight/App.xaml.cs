using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


[assembly: ExportFont("Poppins-SemiBold.ttf", Alias = "Poppins-SemiBold")]
[assembly: ExportFont("Poppins-Bold.ttf", Alias = "Poppins-Bold")]
[assembly: ExportFont("Poppins-Light.ttf", Alias = "Poppins-Light")]
namespace DayVsNight
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
