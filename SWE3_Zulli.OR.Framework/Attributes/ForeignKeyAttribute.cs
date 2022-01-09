using System;

namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>
    /// This attribute marks a member as a foreign key field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : ColumnAttribute
    {
        /// <summary>
        /// Specifies an assignement table for m:n relationships.
        /// </summary>
        /// <remarks>ColumnName dentotes the near foreign key of the assignment table, RemoteColumnName denotes the far key.</remarks>
        public string TargetTableName { get; set; } = null;

        /// <summary>
        /// Specifies the far side foreign key in the assignment table.
        /// </summary>
        public string TargetColumnName { get; set; } = null;
    }
}