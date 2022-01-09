using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Demos
{
    public static class WithForeignKeyTWO
    {
        public static void Show()
        {
            Console.WriteLine("\n[03]Create and Load an Object with Foreign Key WITH CLASS(1:n)");
            Console.WriteLine("*************************************");


            Class c = new()
            {
                ID = 0,
                Name = "TheFirstClass",
                Teacher = ORMapper.Get<Teacher>(01),
            };

            Class c1 = new()
            {
                ID = 1,
                Name = "TheSecondClass",
                Teacher = ORMapper.Get<Teacher>(01),
            };

            ORMapper.Save(c);
            ORMapper.Save(c1);
            Class output = ORMapper.Get<Class>(0);
            Console.WriteLine(output.Name + " " + output.ID + " " );
            Console.WriteLine(ORMapper.Get<Teacher>(output.Teacher.ID).LastName);
            output = ORMapper.Get<Class>(1);
            Console.WriteLine(output.Name + " " + output.ID +  " " + output.Teacher.ID);
            Console.WriteLine(ORMapper.Get<Teacher>(output.Teacher.ID).LastName);

        }
    }
}
