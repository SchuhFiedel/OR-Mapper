using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Demos
{
    public static class WithFK
    {
        public static void Show()
        {
            Console.WriteLine("\n[03]Create and Load an Object with Foreign Key (1:n)");
            Console.WriteLine("*************************************");

            Course c = new()
            {
                ID = 1,
                Name = "TheFirstCourse",
                Teacher = ORMapper.Get<Teacher>(02)
            };

            ORMapper.Save(c);
            Course output = ORMapper.Get<Course>(02);
            Console.WriteLine(output.Name);
            Console.WriteLine(ORMapper.Get<Teacher>(output.Teacher.ID).ToString());
        }
    }
}
