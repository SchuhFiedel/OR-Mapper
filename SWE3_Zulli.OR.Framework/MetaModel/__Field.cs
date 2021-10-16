using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;



namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>This class holds field metadata.</summary>
    internal class __Field
    {
 
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="entity">Parent entity.</param>
        public __Field(__Entity entity)
        {
            Entity = entity;
        }

        /// <summary>Gets the parent entity.</summary>
        public __Entity Entity
        {
            get; private set;
        }

        /// <summary>Gets the field member.</summary>
        public MemberInfo Member
        {
            get; internal set;
        }

        /// <summary>Gets the field type.</summary>
        public Type Type
        {
            get
            {
                if(Member is PropertyInfo) { return ((PropertyInfo) Member).PropertyType; }

                throw new NotSupportedException("Member type not supported.");
            }
        }

        /// <summary>Gets the column name in table.</summary>
        public string ColumnName
        {
            get; internal set;
        }

        /// <summary>Gets the column database type.</summary>
        public Type ColumnType
        {
            get; internal set;
        }

        /// <summary>Gets if the column is a primary key.</summary>
        public bool IsPrimaryKey
        {
            get; internal set;
        } = false;

        /// <summary>Gets if the column is a foreign key.</summary>
        public bool IsForeignKey
        {
            get; internal set;
        } = false;

        /// <summary>Assignment table.</summary>
        public string AssignmentTable
        {
            get; internal set;
        }

        /// <summary>Remote (far side) column name.</summary>
        public string RemoteColumnName
        {
            get; internal set;
        }

        /// <summary>Gets if the field belongs to a m:n relationship.</summary>
        public bool IsManyToMany
        {
            get; internal set;
        }

        /// <summary>Gets if the column is nullable.</summary>
        public bool IsNullable
        {
            get; internal set;
        } = false;

        /// <summary>Gets if the the column is an external foreign key field.</summary>
        public bool IsExternal
        {
            get; internal set;
        } = false;

        /// <summary>Returns a database column type equivalent for a field type value.</summary>
        /// <param name="value">Value.</param>
        /// <returns>Database type representation of the value.</returns>
        public object ToColumnType(object value)
        {
            if(IsForeignKey)
            {
                return Type._GetEntity().PrimaryKey.ToColumnType(Type._GetEntity().PrimaryKey.GetValue(value));
            }

            if(Type == ColumnType) { return value; }

            //Convert Bool to integer Types -> Bools lead to errors in some DBS
            if(value is bool)
            {
                if(ColumnType == typeof(int)) { return (((bool) value) ? 1 : 0); }
                if(ColumnType == typeof(short)) { return (short) (((bool) value) ? 1 : 0); }
                if(ColumnType == typeof(long)) { return (long) (((bool) value) ? 1 : 0); }
            }

            //Convert Enum Types to integer types -> Enums Lead to errors in PostgreSQL
            if(value is Enum)
            {
                if(ColumnType == typeof(int)) { return (int)value; }
                if(ColumnType == typeof(short)) { return (short) ((int)value); }
                if(ColumnType == typeof(long)) { return (long)((int)value); }
            }

            return value;
        }

        /// <summary>Returns a field type equivalent for a database column type value.</summary>
        /// <param name="value">Value.</param>
        /// <returns>Field type representation of the value.</returns>
        public object ToFieldType(object value, ICollection<object> localCache)
        {
            if(IsForeignKey)
            {
                return ORMapper._CreateObject(Type, value, localCache);
            }


            if(Type == typeof(bool))
            {
                if(value is int) { return ((int) value != 0); }
                if(value is short) { return ((short) value != 0); }
                if(value is long) { return ((long) value != 0); }
            }

            if(Type == typeof(short)) { return Convert.ToInt16(value); }
            if(Type == typeof(int)) { return Convert.ToInt32(value); }
            if(Type == typeof(long)) { return Convert.ToInt64(value); }

            if(Type.IsEnum) { return Enum.ToObject(Type, value); }

            return value;
        }

        /// <summary>Gets the field value.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Field value.</returns>
        public object GetValue(object obj)
        {
            if(Member is PropertyInfo) { return ((PropertyInfo) Member).GetValue(obj); }

            throw new NotSupportedException("Member type not supported.");
        }

        /// <summary>Sets the field value.</summary>
        /// <param name="obj">Object.</param>
        /// <param name="value">Value.</param>
        public void SetValue(object obj, object value)
        {
            if(Member is PropertyInfo)
            {
                ((PropertyInfo) Member).SetValue(obj, value);
                return;
            }

            throw new NotSupportedException("Member type not supported.");
        }

        /// <summary>Fills a list for a foreign key.</summary>
        /// <param name="list">List.</param>
        /// <param name="obj">Object.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>List.</returns>
        public object Fill(object list, object obj, ICollection<object> localCache)
        {
            IDbCommand cmd = ORMapper.Connection.CreateCommand();

            if(IsManyToMany)
            {
                cmd.CommandText = Type.GenericTypeArguments[0]._GetEntity().GetSQL() +
                                  " WHERE ID IN (SELECT " + RemoteColumnName + " FROM " + AssignmentTable + " WHERE " + ColumnName + " = :fk)";
            }
            else
            {
                cmd.CommandText = Type.GenericTypeArguments[0]._GetEntity().GetSQL() + " WHERE " + ColumnName + " = :fk";
            }

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":fk";
            p.Value = Entity.PrimaryKey.GetValue(obj);
            cmd.Parameters.Add(p);

            IDataReader re = cmd.ExecuteReader();
            while(re.Read())
            {
                list.GetType().GetMethod("Add").Invoke(list, new object[] { ORMapper._CreateObject(Type.GenericTypeArguments[0], re, localCache) });
            }
            re.Close();
            re.Dispose();
            cmd.Dispose();

            return list;
        }
    }
}
