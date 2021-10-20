using SWE3_Zulli.OR.Framework.MetaModel;
using System;
using System.Collections.Generic;

namespace SWE3_Zulli.OR.Models
{
    /// <summary>This is a teacher implementation (from School example).</summary>
    public class Teacher: Person
    {
        /// <summary>
        /// Gets or Sets Teacher Salary
        /// </summary>
        [Field(ColumnName = "salary")]
        public int Salary { get; set; }

        /// <summary>
        /// Gets or Sets Teacher Hire Date
        /// </summary>
        [Field(ColumnName = "hiredate")]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Gets the Teacher Courses List.
        /// </summary>
        [ForeignKey(ColumnName = "fk_teacher")]
        public List<Course> Courses { get; private set; }


    }
}