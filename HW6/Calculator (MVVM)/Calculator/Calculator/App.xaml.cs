using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Calculator
{
    public partial class App : Application
    {
        ViewModel viewModel;
        public App()
        {
            //InitializeComponent();
            viewModel = new ViewModel();
            viewModel.RestoreState(Current.Properties);
            MainPage = new MainPage(viewModel);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            //viewModel.SaveState(Current.Properties);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
