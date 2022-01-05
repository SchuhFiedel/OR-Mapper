using System;
using System.Collections.Generic;
using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;

namespace SWE3_Zulli.OR.Demos
{
    public static class InsertObject
    {
        public static void Show()
        {
            Console.WriteLine("[01]InsertObject");
            Console.WriteLine("*************");
            /*
            Person max = new()
            {
                ID = 01,
                FirstName = "Max",
                LastName = "Zulli",
                Gender = Gender.MALE,
                BirthDate = DateTime.Parse("13-12-2000")
            };*/

            Teacher teacher = new Teacher()
            {
                ID = 01,
                FirstName = "Uwu",
                LastName = "AAAAAAAA",
                Gender = Gender.FEMALE,
                BirthDate = DateTime.Now,
                HireDate = DateTime.UtcNow,
                Salary = 40000
            };

            //Console.WriteLine(teacher.ToString());
            ORMapper.Save(teacher);

            Person output = ORMapper.Get<Teacher>(01);
            Console.WriteLine($"{output.LastName} has been inserted");
        }
    }
}
