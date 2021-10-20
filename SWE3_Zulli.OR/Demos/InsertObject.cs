using System;
using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;

namespace SWE3_Zulli.OR.Demos
{
    public static class InsertObject
    {
        public static void Show()
        {
            Console.WriteLine("InsertObject");
            Console.WriteLine("*************");

            Person max = new()
            {
                ID = 01,
                FirstName = "Max",
                LastName = "Zulli",
                Gender = Gender.MALE,
                BirthDate = DateTime.Now
            };

            Console.WriteLine(max.ToString());

            ORMapper.Save(max);

            Console.WriteLine("Inserted!");
            Console.WriteLine("*************");
        }
    }
}
