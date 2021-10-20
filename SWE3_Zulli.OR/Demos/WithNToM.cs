using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Demos
{
    public class WithNToM
    {
        public void Show()
        {
            Console.WriteLine("Create and Load an Object with M:N");
            Console.WriteLine("*************************************");

            Course c = new()
            {
                ID = 1,
                Name = "TheFirstCourse",
                Teacher = Framework.ORMapper.Get<Teacher>(1)
            };

            /* Hier fehlt der Großteil weil ich ned mitgekommen bin */


        }
        
    }
}
