using SWE3_Zulli.OR.Framework.Intefaces;
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
        public PropertyInfo Member
        {
            get; internal set;
        }

        /// <summary>Gets the field type.</summary>
        public Type Type
        { 
            get { return Member.PropertyType; } 
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
            if(IsForeignKey == true)
            {
                if (value == null) { return null; }

                //Type type = typeof(ILazy).IsAssignableFrom(Type) ? Type.GenericTypeArguments[0] : Type;

                return Type._GetEntity().PrimaryKey.ToColumnType(Type._GetEntity().PrimaryKey.GetValue(value));
            }

            if(Type == ColumnType) { return value; }

            //Convert Bool to integer Types -> Bools lead to errors in some DBS
            if(value is bool @boolean)
            {
                if(ColumnType == typeof(int)) { return (@boolean ? 1 : 0); }
                if(ColumnType == typeof(short)) { return (short) (@boolean ? 1 : 0); }
                if(ColumnType == typeof(long)) { return (long) (@boolean ? 1 : 0); }
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
                /*if (typeof(ILazy).IsAssignableFrom(Type))
                {
                    return Activator.CreateInstance(Type, value);
                }*/
                return ORMapper._CreateObject(Type, value, localCache);
            }

            if(Type == typeof(bool))
            {
                if(value is int @int) { return (@int != 0); }
                if(value is short @short) { return (@short != 0); }
                if(value is long @long) { return (@long != 0); }
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
            return  Member.GetValue(obj);
        }

        /// <summary>Sets the field value.</summary>
        /// <param name="obj">Object.</param>
        /// <param name="value">Value.</param>
        public void SetValue(object obj, object value)
        {
            Member.SetValue(obj, value);return;
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
                                  " WHERE ID IN (SELECT " + RemoteColumnName + " FROM " + AssignmentTable + " WHERE " + ColumnName + " = @fk)";
            }
            else
            {
                cmd.CommandText = Type.GenericTypeArguments[0]._GetEntity().GetSQL() + " WHERE " + ColumnName + " = @fk";
            }

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = "@fk";
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

        //für n:m 19.10.2021
        public void UpdateReferences(object obj)
        {
            if (!IsExternal) return;

            Type innerType = Type.GetGenericArguments()[0];
            __Entity innerEntity = innerType._GetEntity();

            object pk = Entity.PrimaryKey.ToColumnType(Entity.PrimaryKey.GetValue(obj));

            if (IsManyToMany)
            {
                IDbCommand cmd = ORMapper.Connection.CreateCommand();
                cmd.CommandText = ("DELETE FROM " + AssignmentTable + " WHERE " + ColumnName + " = @pk");
                IDataParameter p = cmd.CreateParameter();

                p.ParameterName = "@pk";
                p.Value = pk;

                cmd.Parameters.Add(p);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                if(GetValue(obj) != null)
                {
                    foreach ( object i in (IEnumerable)GetValue(obj))
                    {
                        cmd = ORMapper.Connection.CreateCommand();
                        cmd.CommandText = "INSERT INTO " + AssignmentTable + 
                            " (" + ColumnName + " , " + RemoteColumnName +
                            ") VALUES ( @pk, @fk)";

                        p.ParameterName = "@pk";
                        p.Value = pk;

                        cmd.Parameters.Add(p);

                        p.ParameterName = "@fk";
                        p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));

                        cmd.Parameters.Add(p);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                else
                {
                    __Field remoteField = innerEntity.GetFieldForColumn(ColumnName);

                    if (remoteField.IsNullable)
                    {
                        try
                        {
                            cmd = ORMapper.Connection.CreateCommand();
                            cmd.CommandText = ("UPDATe " + innerEntity.TableName + " SET " + ColumnName + " = NULL WHERE " + ColumnName + " = @fk");

                            p = cmd.CreateParameter();

                            p.ParameterName = "@fk";
                            p.Value = pk;

                            cmd.Parameters.Add(p);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        catch (Exception) { }
                    }
                    
                    if(GetValue(obj) != null)
                    {
                        foreach(object i in (IEnumerable)GetValue(obj))
                        {
                            remoteField.SetValue(i, obj);

                            cmd = ORMapper.Connection.CreateCommand();
                            cmd.CommandText = ("Update " + innerEntity.TableName + "Set " + ColumnName + " = @fk WHERE " + innerEntity.PrimaryKey.ColumnName + " = @pk");

                            p = cmd.CreateParameter();

                            p.ParameterName = "@fk";
                            p.Value = pk;

                            cmd.Parameters.Add(p);

                            p = cmd.CreateParameter();

                            p.ParameterName = "@pk";
                            p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));

                            cmd.Parameters.Add(p);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
            }
        }
    }
}
