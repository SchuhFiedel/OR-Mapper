using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Demos
{
    public static class WithForeignKeyLIST
    {
        public static void Show()
        {
            Console.WriteLine("\n[04]Create and Load an Object with Foreign Key L I S T(1:n)");
            Console.WriteLine("*************************************");

            Student s = new()
            {
                ID = 2,
                LastName = "fifif",
                FirstName = "uuuuuuu",
                BirthDate = DateTime.Now,
                Gender = Gender.OTHER,
                Grade = 2
            };

            ORMapper.Save(s);

            Course c = new()
            {
                ID = 2,
                Name = "TheThirdCourse",
                Teacher = ORMapper.Get<Teacher>(01),
                Students = new List<Student>()
                {
                    ORMapper.Get<Student>(2)
                }
            };

            ORMapper.Save(c);
            Course output = ORMapper.Get<Course>(0);
            Console.WriteLine(output.Name + " " + output.Teacher.ID);
            Console.WriteLine(ORMapper.Get<Teacher>(output.Teacher.ID).LastName);
        }
    }
}
