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
    public partial class MonthPage : ContentPage
    {
        SQLiteConnection conn;
        WeekPage week;
        public static int month;
        List<string> displayList = new List<string>();
        public MonthPage()
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
            CreateListView();
        }

        private void CreateListView()
        {
            var table = conn.Table<CreateDB.Run>();

            displayList = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                long distance = table.ToList().Where(run => run.Date.Year == TotalsPage.year && run.Date.Month == i).Sum(run => long.Parse(run.Distance));
                displayList.Add(i + ", " + distance);
            }
            lv.ItemsSource = displayList;
            view.InvalidateSurface();
        }

        private void PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            SKPaint paintA = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Blue.ToSKColor(),
                StrokeWidth = 3
            };
            SKPaint paintB = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Red.ToSKColor(),
                TextSize = 45
            };

            // Gets each year's mileage from the listview 
            int total = 0;
            List<float> percList = new List<float>();
            for (int i = 0; i < displayList.Count; i++)
            {
                int mileage = Int32.Parse(displayList[i].Split(',')[1].Trim());
                total += mileage;
            }

            for (int i = 0; i < displayList.Count; i++)
            {
                int mileage = Int32.Parse(displayList[i].Split(',')[1].Trim());
                percList.Add(mileage / (float)total);
            }

            float r = Math.Min(info.Width, info.Height) / 2.0f;
            float barWidth = info.Width * 0.05f;
            List<float> heightList = new List<float>();
            for (int i = 0; i < percList.Count; i++)
            {
                heightList.Add(4f * (info.Height * percList[i]));
            }

            float x = .25f;
            for (int i = 0; i < heightList.Count; i++)
            {
                canvas.DrawRect(x * barWidth, info.Height - heightList[i], barWidth, heightList[i], paintA);
                string month = IntToMonth(displayList[i].Split(',')[0]);
                canvas.DrawText(month, x * barWidth, info.Height - heightList[i] - 15, paintB);
                x += 1.65f;
            }
        }

        private string IntToMonth(string month)
        {
            switch (month)
            {
                case "1":
                    return "Jan";
                case "2":
                    return "Feb";
                case "3":
                    return "Mar";
                case "4":
                    return "Apr";
                case "5":
                    return "May";
                case "6":
                    return "Jun";
                case "7":
                    return "Jul";
                case "8":
                    return "Aug";
                case "9":
                    return "Sep";
                case "10":
                    return "Oct";
                case "11":
                    return "Nov";
                case "12":
                    return "Dec";
                default:
                    return "Invalid";
            }
        }

        public async void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            week = new WeekPage();
            month = Int32.Parse(lv.SelectedItem.ToString().Split(',')[0]);
            await Navigation.PushAsync(week, false);
        }
    }
}