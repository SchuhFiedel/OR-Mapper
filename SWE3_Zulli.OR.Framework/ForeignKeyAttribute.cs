namespace SWE3_Zulli.OR.Framework.MetaModel
{
    public  class ForeignKeyAttribute : FieldAttribute
    {
        /// <summary>Specifies an assignement table for m:n relationships.</summary>
        /// <remarks>ColumnName dentotes the near foreign key of the assignment table, RemoteColumnName denotes the far key.</remarks>
        public string AssignmentTable { get; internal set; } = null;

        /// <summary>Specifies the far side foreign key in the assignment table.</summary>
        public string RemoteColumnName { get; internal set; } = null;
    }
}