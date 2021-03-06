using System;
using SWE3_Zulli.OR.Framework.MetaModel;
using SWE3_Zulli.OR.Demos;
using SWE3_Zulli.OR.Framework;
using System.Reflection;

namespace SWE3_Zulli.OR.Models
{
    [Table(TableName = "person")]
    public class Person
    {
        /// <summary>
        /// Gets or Sets Person ID
        /// </summary>
        [PrimaryKey(ColumnName = "id",Nullable = false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or Sets Persons Last Name
        /// </summary>
        [Column(ColumnName = "lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets Persons First Name
        /// </summary>
        [Column(ColumnName = "firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets Persons Gender
        /// </summary>
        [Column(ColumnName = "gender", ColumnType = typeof(int))]
        public Gender Gender { get; set; }

        
        /// <summary>Gets or sets the person's birth date.</summary>
        [Column(ColumnName = "birthdate")]
        public DateTime BirthDate { get; set; }

        [Ignore]
        public int NumberOfInstances { get; protected set; } = numberOfInstances++;

        protected static int numberOfInstances;

        public string ToString()
        {
            string retval = this.GetType().Name;
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                retval += " " + prop.Name + ": " + prop.GetValue(this) + ";";
            }
            return retval;
        }
    }

    /// <summary>This enumeration defines genders.</summary>
    public enum Gender : int
    {
        FEMALE = 0,
        MALE = 1,
        OTHER = 2
    }

    
}
