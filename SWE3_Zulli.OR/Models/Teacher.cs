using SWE3_Zulli.OR.Framework.MetaModel;
using System;
using System.Collections.Generic;

namespace SWE3_Zulli.OR.Models
{
    [Table(TableName = "teacher")]
    /// <summary>This is a teacher implementation (from School example).</summary>
    public class Teacher: Person
    {
        /// <summary>
        /// Gets or Sets Teacher Salary
        /// </summary>
        [Column(ColumnName = "salary")]
        public int Salary { get; set; }

        /// <summary>
        /// Gets or Sets Teacher Hire Date
        /// </summary>
        [Column(ColumnName = "hiredate")]
        public DateTime HireDate { get; set; }

        

    }
}