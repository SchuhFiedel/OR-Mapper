﻿using System;
using SWE3_Zulli.OR.Framework.MetaModel;
using SWE3_Zulli.OR.Demos;


namespace SWE3_Zulli.OR.Models
{
    class Person
    {
        /// <summary>
        /// Gets or Sets Person ID
        /// </summary>
        [PrimaryKey(ColumnName = "id",Nullable = false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or Sets Persons Last Name
        /// </summary>
        [Field(ColumnName = "lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets Persons First Name
        /// </summary>
        [Field(ColumnName = "firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets Persons Gender
        /// </summary>
        [Field(ColumnName = "gender", ColumnType = typeof(int))]
        public Gender Gender { get; set; }

        
        /// <summary>Gets or sets the person's birth date.</summary>
        [Field(ColumnName = "birthdate")]
        public DateTime BirthDate { get; set; }
    }

    /// <summary>This enumeration defines genders.</summary>
    public enum Gender : int
    {
        FEMALE = 0,
        MALE = 1,
        OTHER = 2
    }
}