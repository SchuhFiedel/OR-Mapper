using System;
using System.Collections.Generic;
using System.Data;
using SWE3_Zulli.OR.Framework.MetaModel;
using Npgsql;
using SWE3_Zulli.OR.Framework.Interfaces;

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

        /// <summary>Gets or sets the locking mechanism used by the framework.</summary>
        public static ILocking Lock { get; set; }

        public static ICache Cache { get; set; }

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
            /*if (_cache.ContainsKey(primaryKey))
            {
                return (Type)_cache.Get(primaryKey);
            }*/
            return (Type)_InstantiateObject(typeof(Type), primaryKey);
        }

        /// <summary>Saves an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Save(object obj)
        {
            if (Cache != null) { if (!Cache.ObjectHasChanged(obj)) return; }

            //Connection.Open();
            Table entity = obj._GetTable();
            //Table ebase = obj.GetType().BaseType._GetTable();

            using (IDbCommand cmd = Connection.CreateCommand())
            {
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
            }
           
            //für n:m 19.10.2021
            foreach (Column field in entity.ExternalFields)
            {
                field.UpdateReferences(obj);
            }
            Connection.Close();
            if (Cache != null) { Cache.PutObject(obj); }
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
        /// <returns>Returns the cached object that matches the current reader or NULL if no such object has been found.</returns>
        internal static object _SearchCache(Type type, object primarykey)
        {
            if (Cache != null && Cache.ContainsObject(type, primarykey))
            {
                return Cache.GetObject(type, primarykey);
            }
            return null;
        }


        /// <summary>Creates an object from a database reader.</summary>
        /// <param name="type">Type.</param>
        /// <param name="reader">Reader.</param>
        /// <returns>Object.</returns>
        internal static object _InstantiateObject(Type type, Dictionary<string, object>columnValuePairs)
        {
            try
            {
            //Connection;
            Table ent = type._GetTable();
            object returnValue = _SearchCache(type, ent.PrimaryKey.ToFieldType(columnValuePairs[ent.PrimaryKey.ColumnName]));
            bool foundincache = true;
            if (returnValue == null)
            {
                foundincache = false;
                returnValue = Activator.CreateInstance(type);
            }

            foreach (Column internalField in ent.InternalFields)
            {
                internalField.SetValue(returnValue, internalField.ToFieldType(columnValuePairs[internalField.ColumnName]));
            }

            if (foundincache == true)
            {
                foreach (Column externalField in ent.ExternalFields)
                {
                    externalField.SetValue(returnValue, externalField.Fill(Activator.CreateInstance(externalField.Type), returnValue));
                }
            }
            if(Cache != null) 
                Cache.PutObject(returnValue);

            Connection.Close();
            return returnValue;
            }
            catch
            {
                throw new ArgumentOutOfRangeException("No Object with this Primary Key was found!");
            }
        }

        /// <summary>
        /// saving dataReaders to DataReaderDictionary because else PGSQL buggs out -> too many open connections
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static Dictionary<string, object> DataReaderToDictionary(IDataReader dataReader, Table entity)
        {
            Dictionary<string, object> columnValuePairs = new();
            //Console.WriteLine(dataReader.ToString());
            if (dataReader.Read())
            {
                
                foreach (Column modelField in entity.InternalFields)
                {
                    columnValuePairs.Add(modelField.ColumnName, dataReader.GetValue(dataReader.GetOrdinal(modelField.ColumnName)));
                }
            }
            return columnValuePairs;
        }

        /// <summary>Creates an instance by its primary keys.</summary>
        /// <param name="type">Type.</param>
        /// <param name="primaryKey">Primary key.</param>
        /// <returns>Object.</returns>
        internal static object _InstantiateObject(Type type, object primaryKey)
        {
            //Connection.Open();
            object returnValue = _SearchCache(type, primaryKey);

            if (returnValue == null)
            {
                using (IDbCommand cmd = Connection.CreateCommand())
                {
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
                    returnValue = _InstantiateObject(type, columnValuePairs);
                }
                
            }
            Connection.Close();

            if (returnValue == null) { throw new Exception("No data."); }
            return returnValue;
        }

        /// <summary>Deletes an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Delete(object obj)
        {
            
            Table ent = obj._GetTable();
            //Table bse = obj.GetType().BaseType._GetTable();

            /*if (bse.IsMaterial)
            {
                _DeleteObject(obj, ent, false);
                _DeleteObject(obj, bse, true);
            }*/
            //else {
                _DeleteObject(obj, ent, true);
            //}
            Connection.Close();
        }

        internal static void _DeleteObject(object primaryKey, Table table, bool isBase)
        {
            using (IDbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = ("DELETE FROM " + table.TableName + " WHERE " + (isBase ? table.PrimaryKey.ColumnName : table.ChildKey) + " = @pk");
                IDataParameter param = cmd.CreateParameter();
                param.ParameterName = "@pk";
                param.Value = table.PrimaryKey.GetValue(primaryKey);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            object delObj = null;
            if (Cache != null)
            {
                delObj = _SearchCache(typeof(Type), primaryKey);
            }
            if(Cache != null && delObj != null)
            {
                Cache.RemoveObject(delObj);
            }
        }

        /// <summary>Locks an object.</summary>
        /// <param name="obj">Object.</param>
        /// <exception cref="LockingException">Thrown when the object could not be locked.</exception>
        /// <exception cref="ObjectLockedException">Thrown when the object is already locked by another instance.</exception>
        public static void LockObject(object obj)
        {
            if (Lock != null) { Lock.Lock(obj); }
        }

        /// <summary>Releases a lock on an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Unlock(object obj)
        {
            if (Lock != null) { Lock.Unlock(obj); }
        }

        /// <summary>
        /// Delete All Locks / Release All Locks
        /// </summary>
        public static void Purge()
        {
            if(Lock != null) { Lock.Purge(); };
        }
    }
}
