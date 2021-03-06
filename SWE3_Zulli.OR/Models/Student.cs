using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Framework.MetaModel;

namespace SWE3_Zulli.OR.Models
{
    /// <summary>This is a student implementation (from School example).</summary>
    [Table(TableName = "Student")]
    public class Student : Person
    {
        /// <summary>
        /// Gets or sets the Student Grade
        /// </summary>
        [Column(ColumnName = "grade")]
        public int Grade { get; set; }

        /*[ForeignKey(ColumnName = "fk_class")]
        public Course Course { get; set; }
        */
    }
}