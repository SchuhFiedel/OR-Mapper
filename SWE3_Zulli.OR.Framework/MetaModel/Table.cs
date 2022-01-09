using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>This class holds metadata for entity.</summary>
    internal class Table
    {
        bool NamesToLowerFlag = true;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="type">Variable/Table Type.</param>
        public Table(Type type)
        {
            //set tablename
            TableAttribute typeattr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));

            if((typeattr == null) || (string.IsNullOrWhiteSpace(typeattr.TableName)))
            {
                if (NamesToLowerFlag) TableName = type.Name.ToLower();
                else TableName = type.Name.ToUpper();
            }
            else { 
                TableName = typeattr.TableName;
                SubsetQuery = typeattr.SubsetQuery;
                ChildKey = typeattr.ChildKey;
            }

            //set Member
            Member = type;

            List<Column> fields = new();

            //get all Properties for the object type and iterate through them 
            foreach(PropertyInfo propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                //if IgnoreAttribute on the property continue to next property
                if((IgnoreAttribute)propInfo.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                //Make a new Column object using this Table object
                Column field = new Column(this); 

                //get field attirbute
                ColumnAttribute fieldattr = (ColumnAttribute)propInfo.GetCustomAttribute(typeof(ColumnAttribute));

                if(fieldattr != null) //if there is a filed attribute
                {
                    if(fieldattr is PrimaryKeyAttribute) //and it is a primaryKey
                    {
                        PrimaryKey = field;
                        field.IsPrimaryKey = true;
                    }

                    //set all informations in field according to PropertyInfo
                    field.ColumnName = fieldattr.ColumnName ?? propInfo.Name;
                    field.ColumnType = fieldattr.ColumnType ?? propInfo.PropertyType;
                    field.IsNullable = fieldattr.Nullable;

                    if(field.IsForeignKey = fieldattr is ForeignKeyAttribute ) //if it is a foreignkey
                    {
                        ForeignKeyAttribute foreignattr = (ForeignKeyAttribute)fieldattr;
                        field.IsExternal = typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType);

                        field.TargetTableName  = foreignattr.TargetTableName;
                        field.TargetColumnName = foreignattr.TargetColumnName;
                        field.IsManyToMany = (!string.IsNullOrWhiteSpace(field.TargetTableName)); //m:n?
                    }
                }
                else
                {
                    if((propInfo.GetGetMethod() == null) || (!propInfo.GetGetMethod().IsPublic)) continue;

                    field.ColumnName = propInfo.Name;
                    field.ColumnType = propInfo.PropertyType;
                }
                field.Member = propInfo;

                fields.Add(field);
            }

            Fields = fields.ToArray();
            InternalFields = fields.Where(m => (!m.IsExternal)).ToArray();
            ExternalFields  = fields.Where(m => m.IsExternal).ToArray();
        }


        /// <summary>Gets the member type.</summary>
        public Type Member
        {
            get; private set;
        }

        /// <summary>Gets the table name.</summary>
        public string TableName
        {
            get; private set;
        }

        /// <summary>Gets the entity fields.</summary>
        public Column[] Fields
        {
            get; private set;
        }

        /// <summary>Gets external fields.</summary>
        /// <remarks>External fields are referenced fields that do not belong to the underlying table.</remarks>
        public Column[] ExternalFields
        {
            get; private set;
        }

        /// <summary>Gets internal fields.</summary>
        public Column[] InternalFields
        {
            get; private set;
        }

        /// <summary>Gets the entity primary key.</summary>
        public Column PrimaryKey
        {
            get; private set;
        }

        /// <summary>Gets the foreign key that references master table.</summary>
        public string ChildKey
        {
            get; private set;
        }

        /// <summary>Gets the subset query.</summary>
        public string SubsetQuery
        {
            get; private set;
        }

        /// <summary>
        /// Gets the Sql querry for a Table.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        /// <returns>SQL string.</returns>
        public string GetSQL(string prefix = null)
        {
            if(prefix == null)
            {
                prefix = "";
            }

            string returnValue = "SELECT ";
            for(int i = 0; i < InternalFields.Length; i++)
            {
                if(i > 0) { returnValue += ", "; }
                returnValue += prefix.Trim() + InternalFields[i].ColumnName;
            }
            returnValue += (" FROM " + TableName);
            return returnValue;
        }

        /// <summary>
        /// Gets a field by its column name.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Field.</returns>
        public Column GetFieldForColumn(string columnName)
        {
            columnName = columnName.ToLower();
            foreach(Column internalField in InternalFields)
            {
                if(internalField.ColumnName.ToLower() == columnName) { return internalField; }
            }
            return null;
        }
    }
}
