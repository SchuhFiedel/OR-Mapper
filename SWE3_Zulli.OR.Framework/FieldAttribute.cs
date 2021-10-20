using System;

namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>This attribute marks a member as a field.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FieldAttribute : Attribute
    {
        /// <summary>Database column name.</summary>
        public string ColumnName  = null;

        /// <summary>Database column type.</summary>
        public Type ColumnType  = null;

        /// <summary>Nullable flag.</summary>
        public bool Nullable  = false;
    }
}