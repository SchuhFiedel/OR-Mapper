using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>
    /// This attribute marks a class as an entity/database table.
    /// </summary>
    
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName;

        /// <summary>
        /// Provides a WHERE-clause that defines a subset of the entity table.
        /// </summary>
        public string SubsetQuery;

        /// <summary>
        /// Foreign key that references master table.
        /// </summary>
        public string ChildKey;

        /// <summary>
        /// Discriminator option -- currently unused
        /// </summary>
        public bool Discriminator = false;
    }
}
