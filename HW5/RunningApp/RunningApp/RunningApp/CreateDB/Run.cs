using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SQLite;
using Xamarin.Forms;

namespace RunningApp.CreateDB
{
    [Table("run")]
    class Run
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Distance { get; set; }
        public DateTime Time { get; set; }
        [Ignore]
        public string DateShortString
        {
            get
            {
                return Date.ToShortDateString();
            }
        }
        [Ignore]
        public string TimeShortString
        {
            get
            {
                return Time.ToString("HH:mm:ss");

            }
        }
        [Ignore]
        public string MilesToKilos
        {
            get
            {
                double miles = Double.Parse(Distance);
                return Math.Round((miles * 1.609), 2) + ""; 
            }
        }
        public override string ToString()
        {
            return String.Format("Distance: {0}, Date: {1}, Time: {2}", Distance, Date.ToString("MM/dd/yyyy"), Time.ToString("HH:mm:ss"));
        }

        /*
 * The following code should be used to populate the database
 * of runs. Do not change this code.
 */
        public static void GenerateRunData(int startYear, int numYears, SQLiteConnection conn)
        {
            const double baseMileage = 3.0;
            for (int dy = 0; dy < numYears; dy++)
            {
                int year = startYear + dy;
                double yearAdjustment = 1.0 + dy * 0.01;
                for (int month = 1; month <= 12; month++)
                {
                    double monthAdjustment = 1.0 + month * 0.02;
                    for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                    {
                        DateTime date = new DateTime(year, month, day);
                        double length = 0;
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                continue;
                                break;
                            case DayOfWeek.Tuesday:
                                length = baseMileage;
                                break;
                            case DayOfWeek.Wednesday:
                                length = 1.5 * baseMileage;
                                break;
                            case DayOfWeek.Thursday:
                                length = 2 * baseMileage;
                                break;
                            case DayOfWeek.Friday:
                                length = 2 * baseMileage;
                                break;
                            case DayOfWeek.Saturday:
                                length = baseMileage;
                                break;
                            case DayOfWeek.Sunday:
                                length = 4 * baseMileage;
                                break;
                        }
                        int runLengthInMiles = (int)Math.Round(yearAdjustment * monthAdjustment * length);
                        int secondsToCompleteRun = runLengthInMiles * 480;      // 8 minutes per mile
                                                                                // Instead of printing, you should insert the run into your database.
                                                                                // Console.WriteLine(date + " " + runLengthInMiles + " " + secondsToCompleteRun);
                        int minTotal = secondsToCompleteRun / 60;
                        int hour = minTotal / 60;
                        int minFinal = minTotal - (60 * hour);
                        int sec = secondsToCompleteRun - (60 * minTotal);

                        DateTime temp = DateTime.Now;
                        DateTime newTime = new DateTime(temp.Year, temp.Month, temp.Day, hour, minFinal, sec);
                        conn.Insert(new Run
                        {
                            Date = date,
                            Distance = runLengthInMiles + "",
                            Time = newTime
                        });
                    }
                }
            }
        }

    }
}
