using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using Xamarin.Essentials;
using SQLite;
using System.IO;

namespace RunningApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventPage : ContentPage
    {
        SQLiteConnection conn;
        //DataTemplate runList;
        public EventPage()
        {
            InitializeComponent();
            this.BackgroundColor = Color.Black;

            string libFolder = FileSystem.AppDataDirectory;
            string fname = System.IO.Path.Combine(libFolder, "RunnerDB.db");
            //File.Delete(fname);
            conn = new SQLiteConnection(fname);
            conn.CreateTable<CreateDB.Men>();
            conn.CreateTable<CreateDB.Women>();
            CreateDB.Men.CreateDB(conn);
            CreateDB.Women.CreateDB(conn);
           // conn.DropTable<CreateDB.Run>();
            conn.CreateTable<CreateDB.Run>();
            
           // CreateDB.Run.GenerateRunData(2016, 5, conn);
            lv.ItemsSource = conn.Table<CreateDB.Run>().ToList();
        }

        protected override void OnAppearing()
        {
            lv.ItemTemplate = RunList();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                if (Device.Idiom != TargetIdiom.Phone)
                {
                    input.HorizontalOptions = LayoutOptions.Center;
                }
                input.Orientation = StackOrientation.Horizontal;
                dateLabel.WidthRequest = 65;
            }
            else
            {
                input.Orientation = StackOrientation.Vertical;
                dateLabel.WidthRequest = 85;
            }
        }

        // Datatemplate for the events listview 
        DataTemplate RunList()
        {
            DataTemplate r = new DataTemplate(() =>
            {
                Label timeLabel = new Label { Text = "Time (HH:mm:ss)", TextColor = Color.CadetBlue};
                Label time = new Label { FontSize = 16, TextColor = Color.CadetBlue };
                time.SetBinding(Label.TextProperty, "TimeShortString");

                Label dateLabel = new Label { Text = "Date", TextColor = Color.Yellow };
                Label date = new Label { FontSize = 16, TextColor = Color.Yellow };
                date.SetBinding(Label.TextProperty, "DateShortString");

                Label distLabel;
                Label dist = new Label { FontSize = 16, TextColor = Color.White };
                if (Preferences.Get("miles", true) == false)
                {
                    dist.SetBinding(Label.TextProperty, "MilesToKilos");
                    distLabel = new Label { Text = "Distance (k)", TextColor = Color.White };
                }
                else
                {
                    dist.SetBinding(Label.TextProperty, "Distance");
                    distLabel = new Label { Text = "Distance (m)", TextColor = Color.White };
                }

                Label idLabel = new Label { Text = "Run ID", TextColor = Color.Green };
                Label id = new Label { FontSize = 16, TextColor = Color.Green };
                id.SetBinding(Label.TextProperty, "Id");


                StackLayout timeLayout = new StackLayout {  };
                StackLayout distLayout = new StackLayout { WidthRequest = 90};
                StackLayout dateLayout = new StackLayout { WidthRequest = 90 };
                StackLayout idLayout = new StackLayout { WidthRequest = 55 };
                StackLayout content = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center};
                distLayout.Children.Add(distLabel);
                distLayout.Children.Add(dist);
                dateLayout.Children.Add(dateLabel);
                dateLayout.Children.Add(date);
                timeLayout.Children.Add(timeLabel);
                timeLayout.Children.Add(time);
                idLayout.Children.Add(idLabel);
                idLayout.Children.Add(id);

                content.Children.Add(distLayout);
                content.Children.Add(dateLayout);
                content.Children.Add(timeLayout);
                content.Children.Add(idLayout);

                return new ViewCell
                {
                    View = content
                };

            });
            return r;
        }

        // Queries the database to add the current user entry 
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            int hour;
            bool tryHour = Int32.TryParse(TimeHour.Text, out hour);

            int min;
            bool tryMin = Int32.TryParse(TimeMin.Text, out min);
            if (!(tryMin && tryHour && (hour < 99) && (hour > -1) && (min < 59) && (min > -1)))
                return;

            double dist;
            bool trydist = Double.TryParse(Dist.Text, out dist);
            if (!(trydist))
                return;

            DateTime temp = DateTime.Now;
            DateTime newTime = new DateTime(temp.Year, temp.Month, temp.Day, hour, min, 0);
            if (Preferences.Get("miles", true) == false)
                dist = dist / 1.609;
            dist = Math.Round(dist, 2);
            conn.Insert(new CreateDB.Run
            {
                Date = Pick.Date,
                Distance = dist + "",
                Time = newTime
            });

            lv.ItemsSource = conn.Table<CreateDB.Run>().ToList();
        }

        // Queries the database to update the selected cell in the listview 
        private void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            if (lv.SelectedItem != null)
            {
                int hour;
                bool tryHour = Int32.TryParse(TimeHour.Text, out hour);

                int min;
                bool tryMin = Int32.TryParse(TimeMin.Text, out min);
                if (!(tryMin && tryHour && (hour < 99) && (hour > -1) && (min < 59) && (min > -1)))
                    return;

                double dist;
                bool trydist = Double.TryParse(Dist.Text, out dist);
                if (!(trydist))
                    return;

                DateTime temp = DateTime.Now;
                DateTime newTime = new DateTime(temp.Year, temp.Month, temp.Day, hour, min, 0);
                if (Preferences.Get("miles", true) == false)
                    dist = dist / 1.609;
                var selectedRun = lv.SelectedItem;
                string[] data = selectedRun.ToString().Split(' ');
                dist = Math.Round(dist, 2);
                conn.Update(new CreateDB.Run
                {
                    Id = Int32.Parse(data[data.Length - 1]),
                    Date = Pick.Date,
                    Distance = dist + "",
                    Time = newTime
                });

                lv.ItemsSource = conn.Table<CreateDB.Run>().ToList();
            }
        }
           
        // Queries the database to delete the selected item in the listview 
        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {

            if (lv.SelectedItem != null)
            {
                var selectedRun = lv.SelectedItem;
                string[] data = selectedRun.ToString().Split(' ');
                conn.Execute("DELETE FROM Run WHERE _id = " + Int32.Parse(data[data.Length - 1]));

                lv.ItemsSource = conn.Table<CreateDB.Run>().ToList();
            }
        }
    }
}