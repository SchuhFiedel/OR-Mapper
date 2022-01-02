using System;
using SWE3_Zulli.OR.Framework;

using SWE3_Zulli.OR.Demos;

namespace SWE3_Zulli.OR
{
    class Program
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // entry point                                                                                                      //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>This is the program main entry point.</summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            //ORMapper.Connection = new SQLiteConnection("Data Source=test.sqlite;Version=3;");
            ORMapper.ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=ORTest;Pooling=true";

            

            InsertObject.Show();

            WithNToM.Show();
            //ModifyObject.Show();
            //WithForeignKey.Show();
            //WithForeignKeyKList.Show();
        }
    }
}
