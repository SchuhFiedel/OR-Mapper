using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Demos
{
    public static class WithNToM
    {
        public static void Show()
        {
            Console.WriteLine("\n[05]Create and Load an Object with M:N");
            Console.WriteLine("*************************************");

            Course c = new Course();
            c.ID = 1;
            c.Name = "TheSecondCourse";
            Teacher tmp = (Teacher)ORMapper.Get<Teacher>(1);
            c.Teacher = tmp;

            Student s = new Student()
            {
                ID = 3,
                FirstName = "Uaua",
                LastName = "Ekeye",
                Gender = Gender.FEMALE,
                BirthDate = DateTime.Now,
                Grade = 2,
            };
            
            ORMapper.Save(s);
            c.Students.Add(s);

            s = new Student()
            {
                ID = 4,
                FirstName = "Oo00k",
                LastName = "asdadd",
                Grade = 1,
                Gender = Gender.OTHER,
                BirthDate = DateTime.UtcNow,
            };

            ORMapper.Save(s);
            c.Students.Add(s);

            ORMapper.Save(c);

            c = ORMapper.Get<Course>(1);

            //Print List of Students
            Console.WriteLine("Students in " + c.Name + ":");
            foreach (Student i in c.Students)
            {
                Console.WriteLine(i.FirstName + " " + i.LastName);
            }
            Console.WriteLine("\n");
        }
        
    }
}
