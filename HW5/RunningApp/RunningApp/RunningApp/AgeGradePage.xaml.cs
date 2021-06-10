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
using System.Reflection;

namespace RunningApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AgeGradePage : ContentPage
    {
        SQLiteConnection conn;
        public AgeGradePage()
        {
            InitializeComponent();
            this.BackgroundColor = Color.Black;

            string libFolder = FileSystem.AppDataDirectory;
            string fname = System.IO.Path.Combine(libFolder, "RunnerDB.db");
            conn = new SQLiteConnection(fname);
            if (Preferences.Get("gender", 0) == 0)
                conn.CreateTable<CreateDB.Men>();
            else
                conn.CreateTable<CreateDB.Women>();
        }

        // Takes a time string in the format mm:ss.ms or mm:ss or HH:mm:ss and converts it to the equivalent double 
        public double GetTimeDouble(string timeString)
        {
            if (timeString.Contains('.') || timeString.IndexOf(':') == timeString.LastIndexOf(':'))
            {
                string[] temp = timeString.Split(':');
                int min = Int32.Parse(temp[0]);
                int sec = (int)(Double.Parse(temp[1]) / 60 * 100);
                string newTime = min + "." + sec;
                return Double.Parse(newTime);
            }
            else if (!(timeString.IndexOf(':') == timeString.LastIndexOf(':')))
            {
                string[] temp = timeString.Split(':');
                int min = Int32.Parse(temp[0]) * 60 + Int32.Parse(temp[1]);
                int sec = (int)(Double.Parse(temp[2]) / 60 * 100);
                string newTime = min + "." + sec;
                return Double.Parse(newTime);
            }
            return 0;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OnEntryChanged(null, null);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                if (Device.Idiom != TargetIdiom.Phone)
                {
                    top.HorizontalOptions = LayoutOptions.Center;
                }
                top.Orientation = StackOrientation.Horizontal;
            }
            else
            {
                top.Orientation = StackOrientation.Vertical;
            }
        }

        public void OnEntryChanged(object sender, EventArgs e)
        {
            int timeHour;
            int timeMin;
            int timeSec;
            bool tryHour = Int32.TryParse(TimeHour.Text, out timeHour);
            bool tryMin = Int32.TryParse(TimeMin.Text, out timeMin);
            bool trySec = Int32.TryParse(TimeSec.Text, out timeSec);
        
            if (Distance.SelectedIndex != -1 && tryHour && tryMin && trySec && timeHour < 100 && timeMin < 60 && timeSec < 60)
            {
                string dist = (string)Distance.SelectedItem;
                int userAge = DateTime.Now.Year - Preferences.Get("dob", DateTime.Now).Year;
                int roundedAge = (int)Math.Floor((double)(userAge / 5)) * 5;
                if (roundedAge < 40)
                    roundedAge = 0;

                // Queries the database for the world record based on the users age and distance 
                if (Preferences.Get("gender", 0) == 0)
                {
                    var query = from men in conn.Table<CreateDB.Men>() where men.Age == roundedAge && dist == men.Distance select men;
                    string timeString = query.ToList()[0].ToString().Split(',')[1];
                    if (timeString == "n/a")
                    {
                        Output.Text = "Estimated age grade: n/a";
                        return;
                    }

                    double recordTime = GetTimeDouble(timeString);
                    // Converts seconds to the equivalent double 
                    double userTime = timeHour * 60 + timeMin + (timeSec / 60 * 100);
                    int ageGrade = (int)(recordTime / userTime * 100);

                    Output.Text = "Estimated age grade: " + ageGrade;
                }
                else
                {
                    var query = from women in conn.Table<CreateDB.Women>() where women.Age == roundedAge && dist == women.Distance select women;
                    string timeString = query.ToList()[0].ToString().Split(',')[1];

                    if (timeString == "n/a")
                    {
                        Output.Text = "Estimated age grade: n/a";
                        return;
                    }

                    double recordTime = GetTimeDouble(timeString);
                    double userTime = timeHour * 60 + timeMin + (timeSec / 60 * 100);
                    int ageGrade = (int)(recordTime / userTime * 100);

                    Output.Text = "Estimated age grade: " + ageGrade;
                }
            }
        }
    }
}