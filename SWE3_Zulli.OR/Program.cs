using System;
using SWE3_Zulli.OR.Framework;

using SWE3_Zulli.OR.Demos;
using SWE3_Zulli.OR.Framework.Cache;
using System.Collections.Generic;

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


            ORMapper.Cache = new TrackerCache();
            InsertObject.Show();
            ModifyObject.Show();
            WithFK.Show();
            //WithForeignKeyKList.Show();
            WithNToM.Show();
            /*var cache = ORMapper.Cache.GetCacheList();
            foreach (var x in cache)
            {
                Type t = x.GetType();
                Console.WriteLine(t);
                foreach (var y in x.Value)
                {
                    t = y.GetType();
                    Console.WriteLine(t);
                }

            }*/
            WithCache.Show();
            
            //WithQuery.Show();
            //WIthLocking.Show();
            
            
            
        }
    }
}
