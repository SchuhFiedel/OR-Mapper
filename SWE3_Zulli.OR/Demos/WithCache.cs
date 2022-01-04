using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Framework.Cache;
using SWE3_Zulli.OR.Models;
using System;

namespace SWE3_Zulli.OR.Demos
{
    /// <summary>This show case demonstrates cache functionality.</summary>
    public static class WithCache
    {
        /// <summary>Implements the show case.</summary>
        public static void Show()
        {
            Console.WriteLine("\n[05]Cache demonstration");
            Console.WriteLine("-----------------------");

            Console.WriteLine("\rWithout cache:");
            _ShowInstances();

            Console.WriteLine("\rWith cache:");
            ORMapper.Cache = new BasicCache();
            _ShowInstances();
        }


        /// <summary>Shows instances.</summary>
        private static void _ShowInstances()
        {
            for (int i = 0; i < 7; i++)
            {
                try{
                    Teacher t = ORMapper.Get<Teacher>(i);
                    Console.WriteLine(t.LastName + " [" + t.ID + "] ");
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
