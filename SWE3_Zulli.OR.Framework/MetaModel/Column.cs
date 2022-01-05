
using SWE3_Zulli.OR.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;



namespace SWE3_Zulli.OR.Framework.MetaModel
{
    /// <summary>This class holds field metadata.</summary>
    internal class Column
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="entity">Parent entity.</param>
        public Column(Table entity)
        {
            Table = entity;
        }

        /// <summary>Gets the parent entity.</summary>
        public Table Table
        {
            get; private set;
        }

        /// <summary>Gets the Column member.</summary>
        public PropertyInfo Member
        {
            get; internal set;
        }

        /// <summary>Gets the Column type.</summary>
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
        public string TargetTableName
        {
            get; internal set;
        }

        /// <summary>Remote (far side) column name.</summary>
        public string TargetColumnName
        {
            get; internal set;
        }

        /// <summary>Gets if the Column belongs to a m:n relationship.</summary>
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

        /// <summary>Returns a database column type equivalent for a Column type value.</summary>
        /// <param name="value">Value.</param>
        /// <returns>Database type representation of the value.</returns>
        public object ToColumnType(object value)
        {
            if(IsForeignKey)
            {
                if (value == null) { return null; }

                return Type._GetTable().PrimaryKey.ToColumnType(Type._GetTable().PrimaryKey.GetValue(value));
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
                return ORMapper._InstantiateObject(Type, value, localCache);
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
            return Member.GetValue(obj);
        }

        /// <summary>Sets the field value.</summary>
        /// <param name="obj">Object.</param>
        /// <param name="value">Value.</param>
        public void SetValue(object obj, object value)
        {
            Member.SetValue(obj, value);
            return;
        }

        /// <summary>Fills a list for a foreign key.</summary>
        /// <param name="list">List.</param>
        /// <param name="obj">Object.</param>
        /// <param name="localCache">Local _cache.</param>
        /// <returns>List.</returns>
        public object Fill(object list, object obj, ICollection<object> localCache)
        {
            using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
            {

                if (IsManyToMany)
                {
                    cmd.CommandText = Type.GenericTypeArguments[0]._GetTable().GetSQL() +
                                      " WHERE ID IN (SELECT " + TargetColumnName + " FROM " + TargetTableName + " WHERE " + ColumnName + " = @fk)";
                }
                else
                {
                    cmd.CommandText = Type.GenericTypeArguments[0]._GetTable().GetSQL() + " WHERE " + ColumnName + " = @fk";
                }

                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = "@fk";
                p.Value = Table.PrimaryKey.GetValue(obj);
                cmd.Parameters.Add(p);

                using (IDataReader re = cmd.ExecuteReader())
                {
                    while (re.Read())
                    {
                        list.GetType().GetMethod("Add").Invoke(list, new object[] { ORMapper._InstantiateObject(Type.GenericTypeArguments[0], re.GetValue(re.GetOrdinal("id")) , localCache) });
                    }
                }
            }
            return list;
        }

        //für n:m 19.10.2021
        public void UpdateReferences(object obj)
        {
            if (!IsExternal) return;

            Type innerType = Type.GetGenericArguments()[0];
            Table innerEntity = innerType._GetTable();

            object pk = Table.PrimaryKey.ToColumnType(Table.PrimaryKey.GetValue(obj));

            if (IsManyToMany)
            {
                using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
                {
                    cmd.CommandText = ("DELETE FROM " + TargetTableName + " WHERE " + ColumnName + " = @pk");
                    IDataParameter p = cmd.CreateParameter();
                    p.ParameterName = "@pk";
                    p.Value = pk;

                    cmd.Parameters.Add(p);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                if (GetValue(obj) != null)
                {
                    foreach (object i in (IEnumerable)GetValue(obj))
                    {
                        using (IDbCommand command = ORMapper.Connection.CreateCommand())
                        {
                            object primary = (object)pk;
                            command.CommandText = "INSERT INTO " + TargetTableName +
                            " (" + ColumnName + " , " + TargetColumnName +
                            ") VALUES ( @pk, @fk)";

                            IDataParameter param = command.CreateParameter();
                            param.ParameterName = "@pk";
                            param.Value = primary;

                            command.Parameters.Add(param);

                            IDataParameter param2 = command.CreateParameter();
                            param2.ParameterName = "@fk";
                            object secondary = (object)innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));
                            param2.Value = secondary;
                            command.Parameters.Add(param2);

                            command.ExecuteNonQuery();
                            command.Dispose();
                        }

                    }
                }
                else
                {
                    Column remoteField = innerEntity.GetFieldForColumn(ColumnName);

                    if (remoteField.IsNullable)
                    {
                        try
                        {
                            using (IDbCommand command = ORMapper.Connection.CreateCommand())
                            {
                                command.CommandText = ("UPDATE " + innerEntity.TableName + " SET " + ColumnName + " = NULL WHERE " + ColumnName + " = @fk");

                                IDataParameter p = command.CreateParameter();

                                p.ParameterName = "@fk";
                                p.Value = pk;

                                command.Parameters.Add(p);

                                command.ExecuteNonQuery();
                                command.Dispose();
                            }
                        }
                        catch (Exception) { }
                    }

                    if (GetValue(obj) != null)
                    {
                        foreach (object i in (IEnumerable)GetValue(obj))
                        {
                            remoteField.SetValue(i, obj);

                            using (IDbCommand command = ORMapper.Connection.CreateCommand())
                            {
                                command.CommandText = ("Update " + innerEntity.TableName + "Set " + ColumnName + " = @fk WHERE " + innerEntity.PrimaryKey.ColumnName + " = @pk");

                                IDataParameter p = command.CreateParameter();

                                p.ParameterName = "@fk";
                                p.Value = pk;

                                command.Parameters.Add(p);

                                p = command.CreateParameter();

                                p.ParameterName = "@pk";
                                p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));

                                command.Parameters.Add(p);

                                command.ExecuteNonQuery();
                                command.Dispose();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Gets the foreign key SQL.</summary>
        internal string _FkSql
        {
            get
            {
                if (IsManyToMany)
                {
                    return Type.GenericTypeArguments[0]._GetTable().GetSQL() +
                           " WHERE ID IN (SELECT " + TargetColumnName + " FROM " + TargetTableName + " WHERE " + ColumnName + " = :fk)";
                }

                return Type.GenericTypeArguments[0]._GetTable().GetSQL() + " WHERE " + ColumnName + " = :fk";
            }
        }
    }
}
