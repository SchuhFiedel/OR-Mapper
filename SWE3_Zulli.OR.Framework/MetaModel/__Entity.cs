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
        bool NamesToLowerFlag = true;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="type">Variable/Entity Type.</param>
        public __Entity(Type type)
        {
            //set tablename
            EntityAttribute typeattr = (EntityAttribute)type.GetCustomAttribute(typeof(EntityAttribute));
            if((typeattr == null) || (string.IsNullOrWhiteSpace(typeattr.TableName)))
            {
                //MAYBE NEED To CHANGE THAT TO "ToLower()"?
                if (NamesToLowerFlag) TableName = type.Name.ToLower();
                else TableName = type.Name.ToUpper();

            }
            else { TableName = typeattr.TableName; }

            //set Member
            Member = type;

            List<__Field> fields = new();

            //get all Properties for the object type and iterate through them 
            foreach(PropertyInfo propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                //if IgnoreAttribute on the property continue to next property
                if((IgnoreAttribute)propInfo.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                //Make a new __Field object using this __Entity object
                __Field field = new __Field(this); 

                //get field attirbute
                FieldAttribute fieldattr = (FieldAttribute)propInfo.GetCustomAttribute(typeof(FieldAttribute));

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

                        field.AssignmentTable  = foreignattr.AssignmentTable;
                        field.RemoteColumnName = foreignattr.RemoteColumnName;
                        field.IsManyToMany = (!string.IsNullOrWhiteSpace(field.AssignmentTable)); //m:n?
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
        public __Field[] Fields
        {
            get; private set;
        }

        /// <summary>Gets external fields.</summary>
        /// <remarks>External fields are referenced fields that do not belong to the underlying table.</remarks>
        public __Field[] ExternalFields
        {
            get; private set;
        }

        /// <summary>Gets internal fields.</summary>
        public __Field[] InternalFields
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
            for(int i = 0; i < InternalFields.Length; i++)
            {
                if(i > 0) { returnValue += ", "; }
                returnValue += prefix.Trim() + InternalFields[i].ColumnName;
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
            foreach(__Field internalField in InternalFields)
            {
                if(internalField.ColumnName.ToUpper() == columnName) { return internalField; }
            }
            return null;
        }
    }
}
