using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Reflection;
using System.IO;
using SQLite;

namespace RunningApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        public static SQLiteConnection conn;
        public MainPage()
        {
            InitializeComponent();
            this.BarBackgroundColor = Color.Red;
            string DBName = "RunnerDB.db";
            string libFolder = FileSystem.AppDataDirectory;
            string dest = System.IO.Path.Combine(libFolder, DBName);

            //CreateDB.CreateDB.Main(dest);

            conn = new SQLiteConnection(dest);
        }
    }
}
