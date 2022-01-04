using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Demos
{
    public static class ModifyObject
    {
        public static void Show()
        {
            Console.WriteLine("\n[02]ModifyObject");
            Console.WriteLine("*************");

            Teacher teacher = new Teacher()
            {
                ID = 01,
                FirstName = "Ecece",
                LastName = "AAAAAAAA",
                Gender = Gender.OTHER,
                BirthDate = DateTime.Now,
                HireDate = DateTime.UtcNow,
                Salary = 40000
            };

            Console.WriteLine(teacher.ToString());

            ORMapper.Save(teacher);

            //SalaryTest
            Teacher output = ORMapper.Get<Teacher>(01);
            Console.WriteLine($"{output.LastName} has a salary of {output.Salary}");

            teacher.Salary = 1111111;
            ORMapper.Save(teacher);

            output = ORMapper.Get<Teacher>(01);
            Console.WriteLine($"{output.LastName} NOW has a salary of {output.Salary}");
        }
    }
}



