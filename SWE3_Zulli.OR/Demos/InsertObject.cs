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

            Person max = new Person();

            max.ID = 01;
            max.FirstName = "Max";
            max.LastName = "Zulli";
            max.Gender = Gender.MALE;
            max.BirthDate = DateTime.Now;

            ORMapper.Save(max);

            Console.WriteLine("Inserted!");
            Console.WriteLine("*************");
        }
    }
}
