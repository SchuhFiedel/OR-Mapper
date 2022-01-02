using System;
using System.Collections.Generic;
using System.Data;
using SWE3_Zulli.OR.Framework.MetaModel;
using Npgsql;

namespace SWE3_Zulli.OR.Framework
{
    /// <summary>
    /// OR-MapperClass for basic database manipulation
    /// Save
    /// Get
    /// 
    /// </summary>
    public static class ORMapper
    {
        public static string ConnectionString = null;

        /// <summary>Entities.</summary>
        private static Dictionary<Type, Table> _EntitiesDict = new();

        /// <summary>Caches objects to reduce load</summary>
        private static ICollection<object> cache = new List<object>();

        /// <summary>Gets or sets the database connection used by the framework.</summary>
        public static IDbConnection Connection {
            get 
            { 
                if(ConnectionString == null)
                {
                    throw new NoNullAllowedException("Please add ConnectionString");
                }
                var newcon = new NpgsqlConnection(ConnectionString);
                newcon.Open();
                return newcon;
            } 
            set
            {
                Connection = value;
            }
        }

        /*public static void CreateDBIfNotExists()
        {
            Repositories.Repo repo = new Repositories.Repo();
            repo.Connection = Connection;
            repo.CreateDatabase();
        }*/

        /// <summary>Gets an object.</summary>
        /// <typeparam name="Type">Type.</typeparam>
        /// <param name="primaryKey">Primary key.</param>
        /// <returns>Object.</returns>
        public static Type Get<Type>(object primaryKey)
        {
            /*if (cache.ContainsKey(primaryKey))
            {
                return (Type)cache.Get(primaryKey);
            }*/
            return (Type)_InstantiateObject(typeof(Type), primaryKey, cache);
        }

        /// <summary>Saves an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Save(object obj)
        {
            //Connection.Open();
            Table entity = obj._GetTable();
            Table ebase = obj.GetType().BaseType._GetTable();

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + entity.TableName + " (");

            string update = "ON CONFLICT (" + entity.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";

            IDataParameter param;
            bool first = true;
            for (int i = 0; i < entity.InternalFields.Length; i++)
            {
                if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += entity.InternalFields[i].ColumnName;

                insert += ("@v" + i.ToString());

                param = cmd.CreateParameter();
                param.ParameterName = ("@v" + i.ToString());
                param.Value = entity.InternalFields[i].ToColumnType(entity.InternalFields[i].GetValue(obj));
                cmd.Parameters.Add(param);

                if (!entity.InternalFields[i].IsPrimaryKey)
                {
                    if (first) { first = false; } else { update += ", "; }
                    update += (entity.InternalFields[i].ColumnName + " = " + ("@x" + i.ToString()));

                    param = cmd.CreateParameter();
                    param.ParameterName = ("@x" + i.ToString());
                    param.Value = entity.InternalFields[i].ToColumnType(entity.InternalFields[i].GetValue(obj));

                    cmd.Parameters.Add(param);
                }
            }
            cmd.CommandText += (") VALUES (" + insert + ") " + update);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            //für n:m 19.10.2021
            foreach (Column field in entity.ExternalFields)
            {
                field.UpdateReferences(obj);
            }
            ORMapper.Connection.Close();
        }

        /// <summary>Gets an entity descriptor for an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Table.</returns>
        internal static Table _GetTable(this object obj)
        {
            Type type = null;
            if (obj is Type @otype) type = @otype;
            else
                type = obj.GetType();
            
            if (!_EntitiesDict.ContainsKey(type))
                _EntitiesDict.Add(type, new Table(type));

            return _EntitiesDict[type];
        }

        /// <summary>Searches the cached objects for an object and returns it.</summary>
        /// <param name="type">Type.</param>
        /// <param name="primarykey">Primary key.</param>
        /// <param name="localCache">Cached objects.</param>
        /// <returns>Returns the cached object that matches the current reader or NULL if no such object has been found.</returns>
        internal static object _SearchCache(Type type, object primarykey, ICollection<object> localCache)
        {
            if (localCache != null)
            {
                foreach (object i in localCache)
                {
                    if (i.GetType() != type)
                        continue;
                    if (type._GetTable().PrimaryKey.GetValue(i).Equals(primarykey))
                        return i;
                }
            }
            return null;
        }


        /// <summary>Creates an object from a database reader.</summary>
        /// <param name="type">Type.</param>
        /// <param name="reader">Reader.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>Object.</returns>
        internal static object _InstantiateObject(Type type, Dictionary<string, object>columnValuePairs, ICollection<object> localCache)
        {
            //Connection;
            Table ent = type._GetTable();
            object returnValue = _SearchCache(type, ent.PrimaryKey.ToFieldType(columnValuePairs[ent.PrimaryKey.ColumnName], localCache),localCache);

            if (returnValue == null)
            {
                if (localCache == null) 
                { 
                    localCache = new List<object>(); 
                }
                localCache.Add(returnValue = Activator.CreateInstance(type));
            }

            foreach (Column internalField in ent.InternalFields)
            {
                internalField.SetValue(returnValue, internalField.ToFieldType(columnValuePairs[internalField.ColumnName],localCache));
            }

            foreach (Column externalField in ent.ExternalFields)
            {
                externalField.SetValue(returnValue, externalField.Fill(Activator.CreateInstance(externalField.Type), returnValue, localCache));
            }
            Connection.Close();
            return returnValue;
            
        }

        private static Dictionary<string, object> DataReaderToDictionary(IDataReader dataReader, Table entity)
        {
            Dictionary<string, object> columnValuePairs = new();
            Console.WriteLine(dataReader.ToString());
            if (dataReader.Read())
            {
                
                foreach (Column modelField in entity.Fields)
                {
                    columnValuePairs.Add(modelField.ColumnName, dataReader.GetValue(dataReader.GetOrdinal(modelField.ColumnName)));
                }
            }
            return columnValuePairs;
        }

        /// <summary>Creates an instance by its primary keys.</summary>
        /// <param name="type">Type.</param>
        /// <param name="primaryKey">Primary key.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>Object.</returns>
        internal static object _InstantiateObject(Type type, object primaryKey, ICollection<object> localCache)
        {
            //Connection.Open();
            object returnValue = _SearchCache(type, primaryKey, localCache);

            if (returnValue == null)
            {
                IDbCommand cmd = Connection.CreateCommand();
                Table table = type._GetTable();
                cmd.CommandText = table.GetSQL() + " WHERE " + table.PrimaryKey.ColumnName + " = @pk";

                IDataParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = ("@pk");
                parameter.Value = primaryKey;
                cmd.Parameters.Add(parameter);

                IDataReader re = cmd.ExecuteReader();
                Dictionary<string, object> columnValuePairs = DataReaderToDictionary(re, table);
                re.Close();
                cmd.Dispose();
                returnValue = _InstantiateObject(type, columnValuePairs, localCache);
            }
            Connection.Close();

            if (returnValue == null) { throw new Exception("No data."); }
            return returnValue;
        }

        /// <summary>Deletes an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Delete(object obj)
        {
            //Connection.Open();
            Table ent = obj._GetTable();
            Table bse = obj.GetType().BaseType._GetTable();

            /*if (bse.IsMaterial)
            {
                _DeleteObject(obj, ent, false);
                _DeleteObject(obj, bse, true);
            }*/
            //else {
                _DeleteObject(obj, ent, true, cache);
            //}
            Connection.Close();
        }

        internal static void _DeleteObject(object primaryKey, Table table, bool isBase, ICollection<object> cache)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("DELETE FROM " + table.TableName + " WHERE " + (isBase ? table.PrimaryKey.ColumnName : table.ChildKey) + " = :pk");
            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":pk";
            p.Value = table.PrimaryKey.GetValue(primaryKey);
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            object delObj = _SearchCache(typeof(Type), primaryKey, cache);
            cache.Remove(delObj);
        }
    }
}
