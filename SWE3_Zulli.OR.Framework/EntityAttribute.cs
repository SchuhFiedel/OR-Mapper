using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework
{
    /// <summary>This attribute marks a class as an entity.</summary>
    
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        /// <summary>Table name.</summary>
        public string TableName;
    }
}
