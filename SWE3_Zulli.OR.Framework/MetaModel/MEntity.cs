using System;
using System.Collections.Generic;
using System.Reflection;



namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>This class holds entity metadata.</summary>
    internal class MEntity
    {
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="type">Type.</param>
        public MEntity(Type type)
        {
            EntityAttribute tattr = (EntityAttribute)type.GetCustomAttribute(typeof(EntityAttribute));
            if((tattr == null) || (string.IsNullOrWhiteSpace(tattr.TableName)))
            {
                TableName = type.Name.ToUpper();
            }
            else { TableName = tattr.TableName; }

            Member = type;
            List<MField> fields = new List<MField>();

            foreach(PropertyInfo pInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if((IgnoreAttribute) pInfo.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                MField field = new MField(this);

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
                        field.IsForeignKey = true;
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
        public MField[] Fields
        {
            get; private set;
        }

        /// <summary>Gets the entity primary key.</summary>
        public MField PrimaryKey
        {
            get; private set;
        }
    }
}
