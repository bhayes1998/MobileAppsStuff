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
    public partial class TotalsPage : ContentPage
    {
        public SQLiteConnection conn;
        MonthPage month;
        public static int year;
        List<string> displayList = new List<string>();
        public TotalsPage()
        {
            InitializeComponent();
            this.BackgroundColor = Color.Black;

            string libFolder = FileSystem.AppDataDirectory;
            string fname = System.IO.Path.Combine(libFolder, "RunnerDB.db");
            conn = new SQLiteConnection(fname);

            conn.CreateTable<CreateDB.Run>();
            CreateListView();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
        }

        private void CreateListView()
        {
            var table = conn.Table<CreateDB.Run>();
            var dataList = table.OrderBy(s => s.Date).ToList();
            //lv.ItemsSource = dataList;

            var yearList = (from run in table select run.Date.Year).Distinct();

            displayList = new List<string>();
            foreach (var year in yearList)
            {
                long distance= table.ToList().Where(run => run.Date.Year == year).Sum(run => long.Parse(run.Distance));
                displayList.Add(year + ", " + distance);
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
                StrokeWidth = 3,
                TextSize = 55
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
                heightList.Add(3 * (info.Height * percList[i]));
            }

            float x = .5f;
            for (int i = 0; i < heightList.Count; i++)
            {
                canvas.DrawRect(x * barWidth, info.Height - heightList[i], barWidth, heightList[i], paintA);
                string year = displayList[i].Split(',')[0];
                canvas.DrawText(year, x * barWidth, (info.Height - heightList[i]) - 15, paintB);
                x += 2f;
            }
        }

        public async void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            month = new MonthPage();
            year = Int32.Parse(lv.SelectedItem.ToString().Split(',')[0]);
            await Navigation.PushAsync(month, false);
        }
    }
}