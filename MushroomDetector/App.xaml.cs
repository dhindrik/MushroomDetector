using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MushroomDetector
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Bootstrapper.Init(this);

            MainPage = new MainShell();
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
