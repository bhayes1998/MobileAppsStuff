/*using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using SQLite;

namespace RunningApp.CreateDB
{
    class CreateDB
    {

        public static void Main(string fname)
        {
            File.Delete(fname);

            SQLiteConnection conn = new SQLiteConnection(fname);

            Men.CreateDB(conn);
            Women.CreateDB(conn);
        }
    }
}
*/