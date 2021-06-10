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
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace RunningApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayInfo : ContentPage
    {
        SQLiteConnection conn;
        public DayInfo()
        {
            InitializeComponent();
            this.BackgroundColor = Color.Black;
            string libFolder = FileSystem.AppDataDirectory;
            string fname = System.IO.Path.Combine(libFolder, "RunnerDB.db");
            conn = new SQLiteConnection(fname);

            conn.CreateTable<CreateDB.Run>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var table = conn.Table<CreateDB.Run>();

            var runData = table.ToList().Where(run => run.Date.Year == TotalsPage.year && 
                run.Date.Month == MonthPage.month && run.Date.Day == DayPage.date.Day).Select(run => run.ToString());

            lab.ItemsSource = runData;
        }
    }

    
}