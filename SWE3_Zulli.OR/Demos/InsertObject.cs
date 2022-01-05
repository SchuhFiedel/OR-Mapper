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

            ORMapper.Save(teacher);

            Person output = ORMapper.Get<Teacher>(01);
            Console.WriteLine($"{output.LastName} has been inserted");
        }
    }
}
