using System.Collections.Generic;
using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Framework.MetaModel;

namespace SWE3_Zulli.OR.Models
{

    [Table(TableName = "course")]
    public class Course
    {
        /// <summary>Gets or sets the course ID.</summary>
        [PrimaryKey]
        public int ID { get; internal set; }

        /// <summary>Gets or sets the course name.</summary>
        [Column(ColumnName = "name")]
        public string Name { get; internal set; }

        /// <summary>Gets or sets the course teacher.</summary>
        [ForeignKey(ColumnName = "fk_teacher")]
        public Teacher Teacher { get; internal set; }
        
        /*/// <summary>Gets or sets the courses students..</summary>
        [ForeignKey(ColumnName = "fk_student")]
        public List<Student> Students { get; internal set; }*/
    }
}