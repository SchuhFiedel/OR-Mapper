
using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Demos;
using SWE3_Zulli.OR.Framework.Cache;

namespace SWE3_Zulli.OR
{
    class Program
    {
        /// <summary>
        /// This is the program main entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            //ORMapper.Connection = new SQLiteConnection("Data Source=test.sqlite;Version=3;");
            ORMapper.ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=ORTest;Pooling=true;Include Error Detail=true";


            ORMapper.Cache = new TrackerCache();

            InsertObject.Show();

            ModifyObject.Show();

            WithFK.Show();

            WithForeignKeyTWO.Show();

            //WithNToM includes List
            WithNToM.Show();

            WithCache.Show();

            //WithQuery.Show();

            WithLocking.Show();

        }
    }
}
