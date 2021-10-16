using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>This class holds metadata for entity.</summary>
    internal class __Entity
    {
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="type">Variable/Entity Type.</param>
        public __Entity(Type type)
        {
            EntityAttribute tattr = (EntityAttribute)type.GetCustomAttribute(typeof(EntityAttribute));
            if((tattr == null) || (string.IsNullOrWhiteSpace(tattr.TableName)))
            {
                TableName = type.Name.ToUpper();
            }
            else { TableName = tattr.TableName; }

            Member = type;
            List<__Field> fields = new List<__Field>();

            foreach(PropertyInfo pInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if((IgnoreAttribute)pInfo.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                __Field field = new __Field(this);

                FieldAttribute fattr = (FieldAttribute)pInfo.GetCustomAttribute(typeof(FieldAttribute));

                if(fattr != null)
                {
                    if(fattr is PrimaryKeyAttribute)
                    {
                        PrimaryKey = field;
                        field.IsPrimaryKey = true;
                    }

                    field.ColumnName = (fattr?.ColumnName ?? pInfo.Name);
                    field.ColumnType = (fattr?.ColumnType ?? pInfo.PropertyType);

                    field.IsNullable = fattr.Nullable;

                    if(field.IsForeignKey = (fattr is ForeignKeyAttribute))
                    {
                        field.IsExternal = typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType);

                        field.AssignmentTable  = ((ForeignKeyAttribute) fattr).AssignmentTable;
                        field.RemoteColumnName = ((ForeignKeyAttribute) fattr).RemoteColumnName;
                        field.IsManyToMany = (!string.IsNullOrWhiteSpace(field.AssignmentTable));
                    }
                }
                else
                {
                    if((pInfo.GetGetMethod() == null) || (!pInfo.GetGetMethod().IsPublic)) continue;

                    field.ColumnName = pInfo.Name;
                    field.ColumnType = pInfo.PropertyType;
                }
                field.Member = pInfo;

                fields.Add(field);
            }

            Fields = fields.ToArray();
            Internals = fields.Where(m => (!m.IsExternal)).ToArray();
            Externals  = fields.Where(m => m.IsExternal).ToArray();
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
        public __Field[] Fields
        {
            get; private set;
        }

        /// <summary>Gets external fields.</summary>
        /// <remarks>External fields are referenced fields that do not belong to the underlying table.</remarks>
        public __Field[] Externals
        {
            get; private set;
        }

        /// <summary>Gets internal fields.</summary>
        public __Field[] Internals
        {
            get; private set;
        }

        /// <summary>Gets the entity primary key.</summary>
        public __Field PrimaryKey
        {
            get; private set;
        }

        /// <summary>Gets the entity SQL.</summary>
        /// <param name="prefix">Prefix.</param>
        /// <returns>SQL string.</returns>
        public string GetSQL(string prefix = null)
        {
            if(prefix == null)
            {
                prefix = "";
            }

            string returnValue = "SELECT ";
            for(int i = 0; i < Internals.Length; i++)
            {
                if(i > 0) { returnValue += ", "; }
                returnValue += prefix.Trim() + Internals[i].ColumnName;
            }
            returnValue += (" FROM " + TableName);
            return returnValue;
        }

        /// <summary>Gets a field by its column name.</summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Field.</returns>
        public __Field GetFieldForColumn(string columnName)
        {
            columnName = columnName.ToUpper();
            foreach(__Field internalField in Internals)
            {
                if(internalField.ColumnName.ToUpper() == columnName) { return internalField; }
            }
            return null;
        }
    }
}
