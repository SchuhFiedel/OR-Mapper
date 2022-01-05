using SWE3_Zulli.OR.Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Models
{
    public class Class
    {
        public Class()
        {
            //Students = new List<Student>();
        }

        [PrimaryKey(ColumnName = "id")]
        public int ID { get; set; }

        public string Name { get; set; }

        /*[ForeignKey(ColumnName="student", TargetTableName = "student", TargetColumnName ="fk_class")]
        public List<Student> Students { get; set; }
        */
        [ForeignKey(ColumnName = "fk_teacher")]
        public Teacher Teacher { get; set; }

    }
}
