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
    public partial class WeekPage : ContentPage
    {
        SQLiteConnection conn;
        List<string> displayList = new List<string>();
        DayPage day;
        public static DateTime week;
        public WeekPage()
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
            var days = DateTime.DaysInMonth(TotalsPage.year, MonthPage.month);
            DateTime firstOfMonth = new DateTime(TotalsPage.year, MonthPage.month, 1);
            DateTime secondWeek = new DateTime(TotalsPage.year, MonthPage.month, 8);
            DateTime thirdWeek = new DateTime(TotalsPage.year, MonthPage.month, 15);
            DateTime fourthWeek = new DateTime(TotalsPage.year, MonthPage.month, 22);
            DateTime fifthWeek = new DateTime();
            if (MonthPage.month != 2)
                fifthWeek = new DateTime(TotalsPage.year, MonthPage.month, 29);
            List<string> weeks = new List<string>();
            weeks.Add(firstOfMonth.ToShortDateString());
            weeks.Add(secondWeek.ToShortDateString());
            weeks.Add(thirdWeek.ToShortDateString());
            weeks.Add(fourthWeek.ToShortDateString());
            if (MonthPage.month != 2)
                weeks.Add(fifthWeek.ToShortDateString());

            var table = conn.Table<CreateDB.Run>();
            displayList = new List<string>();
            foreach (string day in weeks)
            {
                DateTime date = DateTime.Parse(day);
                int dayNum = date.Day;
                long distance = 0;
                if (dayNum == 29)
                {
                    for (int i = 29; i <= DateTime.DaysInMonth(TotalsPage.year, MonthPage.month); i++)
                    {
                        distance += table.ToList().Where(run => run.Date.Year == TotalsPage.year && run.Date.Month == MonthPage.month && run.Date.Day == i).Sum(run => long.Parse(run.Distance));
                    }
                }
                else
                {
                    for (int i = dayNum; i < dayNum + 7; i++)
                    {
                        distance += table.ToList().Where(run => run.Date.Year == TotalsPage.year && run.Date.Month == MonthPage.month && run.Date.Day == i).Sum(run => long.Parse(run.Distance));
                    }
                }
                displayList.Add(day + ", " + distance);
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
                TextSize = 65
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
            float barWidth = info.Width * 0.1f;
            List<float> heightList = new List<float>();
            for (int i = 0; i < percList.Count; i++)
            {
                heightList.Add(1.5f * (info.Height * percList[i]));
            }

            float x = .5f;
            for (int i = 0; i < heightList.Count; i++)
            {
                canvas.DrawRect(x * barWidth, info.Height - heightList[i], barWidth, heightList[i], paintA);
                DateTime date = DateTime.Parse(displayList[i].Split(',')[0]);
                canvas.DrawText(date.ToString("MM/dd"), x * barWidth, info.Height - heightList[i] - 15, paintB);
                x += 2f;
            }
        }

        public async void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            day = new DayPage();
            week = DateTime.Parse(lv.SelectedItem.ToString().Split(',')[0]);
            await Navigation.PushAsync(day, false);
        }
    }
}