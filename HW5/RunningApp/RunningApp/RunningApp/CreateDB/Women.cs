using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using SQLite;
using Xamarin.Forms;
using System.Reflection;

namespace RunningApp.CreateDB
{
    class Women
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Distance { get; set; }
        public string Time { get; set; }
        public int Age { get; set; }
        public override string ToString()
        {
            return String.Format("{0},{1},{2}", Distance, Time.ToString(), Age);
        }

        public static void CreateDB(SQLiteConnection conn)
        {

            conn.CreateTable<Women>();

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            string namespaceName = "RunningApp";
            Stream stream = assembly.GetManifestResourceStream(namespaceName + "." + "women.txt");
            string text = "";
            List<string[]> data = new List<string[]>();
            using (var dictReader = new System.IO.StreamReader(stream))
            {
                while ((text = dictReader.ReadLine()) != null)
                {
                    data.Add(text.Split(','));
                }
            }
            data[0][1] = "0";

            for (int i = 1; i < 15; i++)
            {
                conn.Insert(new Women
                {
                    Distance = data[1][0],
                    Time = data[1][i],
                    Age = Int32.Parse(data[0][i])
                });
                conn.Insert(new Women
                {
                    Distance = data[2][0],
                    Time = data[2][i],
                    Age = Int32.Parse(data[0][i])
                });
                conn.Insert(new Women
                {
                    Distance = data[3][0],
                    Time = data[3][i],
                    Age = Int32.Parse(data[0][i])
                });
                conn.Insert(new Women
                {
                    Distance = data[4][0],
                    Time = data[4][i],
                    Age = Int32.Parse(data[0][i])
                });
                conn.Insert(new Women
                {
                    Distance = data[5][0],
                    Time = data[5][i],
                    Age = Int32.Parse(data[0][i])
                });
            }

        }
    }
}
