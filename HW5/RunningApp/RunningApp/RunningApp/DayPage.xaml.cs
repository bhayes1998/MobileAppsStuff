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
    public partial class DayPage : ContentPage
    {
        DayInfo dayInfo;
        SQLiteConnection conn;
        public static DateTime date;
        List<string> displayList = new List<string>();
        public DayPage()
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
            int month = MonthPage.month;
            int i = WeekPage.week.Day;
            int limit;
            if (i == 29)
                limit = DateTime.DaysInMonth(TotalsPage.year, month)+1;
            else
                limit = WeekPage.week.Day + 7;
            DateTime date = WeekPage.week;
            while (i < limit)
            {
                long distance = table.ToList().Where(run => run.Date.Year == TotalsPage.year && run.Date.Month == MonthPage.month && run.Date.Day == i).Sum(run => long.Parse(run.Distance));

                displayList.Add(date.ToShortDateString() + ", " + distance);
                i++;
                if (date.Day == DateTime.DaysInMonth(TotalsPage.year, month))
                    break;
                date = new DateTime(date.Year, date.Month, date.Day + 1);
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
            float barWidth = info.Width * 0.08f;
            List<float> heightList = new List<float>();
            for (int i = 0; i < percList.Count; i++)
            {
                heightList.Add((info.Height * percList[i]));
            }

            float x = .5f;
            for (int i = 0; i < heightList.Count; i++)
            {
                canvas.DrawRect(x * barWidth, info.Height - heightList[i], barWidth, heightList[i], paintA);
                DateTime date = DateTime.Parse(displayList[i].Split(',')[0]);
                canvas.DrawText(date.ToString("MM/dd"), x * barWidth, info.Height - heightList[i] - 15, paintB);
                x += 1.75f;
            }

        }

        public async void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            dayInfo = new DayInfo();
            date = DateTime.Parse(lv.SelectedItem.ToString().Split(',')[0]);
            await Navigation.PushAsync(dayInfo, false);
        }
    }
}