using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Windows.Input;

namespace RunningApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.BackgroundColor = Color.Black;
            BindingContext = this;

            var temp = Preferences.Get("gender", 0);
            GenderPick.SelectedIndex = temp;
            Birthday.Date = Preferences.Get("dob", new DateTime());
            MilesSwitch.IsToggled = Preferences.Get("miles", true);
        }

        public void OnPickerChange(object sender, EventArgs e)
        {
            Picker pick = sender as Picker;
            Preferences.Set("gender", pick.SelectedIndex);
        }

        public void OnDatePickerChange(object sender, EventArgs e)
        {
            DatePicker pick = sender as DatePicker;
            Preferences.Set("dob", pick.Date);
        }

        public void OnMilesSwitchChange(object sender, EventArgs e)
        {
            Xamarin.Forms.Switch s = sender as Xamarin.Forms.Switch;
            Preferences.Set("miles", s.IsToggled);
        }

        public ICommand OpenLink => new Command<string>((url) =>
        {
            Device.OpenUri(new System.Uri(url));
        });

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                if (Device.Idiom != TargetIdiom.Phone)
                {
                    image.WidthRequest = 400;
                    image.HeightRequest = 400;
                    top.HorizontalOptions = LayoutOptions.Center;
                }
                else
                {
                    bottom.Orientation = StackOrientation.Horizontal;
                }
                top.Orientation = StackOrientation.Horizontal;
                bottom.HorizontalOptions = LayoutOptions.Center;
            }
            else
            {
                top.Orientation = StackOrientation.Vertical;
                bottom.Orientation = StackOrientation.Vertical;
            }
        }
    }
}