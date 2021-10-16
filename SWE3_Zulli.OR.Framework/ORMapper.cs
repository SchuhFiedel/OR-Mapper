using System;
using System.Collections.Generic;
using System.Data;
using SWE3_Zulli.OR.Framework.MetaModel;

namespace SWE3_Zulli.OR.Framework
{
    public static class ORMapper
    {
        /// <summary>Entities.</summary>
        private static Dictionary<Type, __Entity> _Entities = new Dictionary<Type, __Entity>();

        /// <summary>Gets or sets the database connection used by the framework.</summary>
        public static IDbConnection Connection { get; set; }

        /// <summary>Gets an object.</summary>
        /// <typeparam name="Type">Type.</typeparam>
        /// <param name="primaryKey">Primary key.</param>
        /// <returns>Object.</returns>
        public static Type Get<Type>(object primaryKey)
        {
            return (Type)_CreateObject(typeof(Type), primaryKey, null);
        }

        /// <summary>Gets an entity descriptor for an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Entity.</returns>
        internal static __Entity _GetEntity(this object obj)
        {
            Type type = ((obj is Type) ? (Type)obj : obj.GetType());

            if (!_Entities.ContainsKey(type))
            {
                _Entities.Add(type, new __Entity(type));
            }

            return _Entities[type];
        }

        /// <summary>Saves an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Save(object obj)
        {
            __Entity ent = obj._GetEntity();

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + ent.TableName + " (");

            string update = "ON CONFLICT (" + ent.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";

            IDataParameter param;
            bool first = true;
            for (int i = 0; i < ent.Internals.Length; i++)
            {
                if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += ent.Internals[i].ColumnName;

                insert += ("@v" + i.ToString());

                param = cmd.CreateParameter();
                param.ParameterName = ("@v" + i.ToString());
                param.Value = ent.Internals[i].ToColumnType(ent.Internals[i].GetValue(obj));
                cmd.Parameters.Add(param);

                if (!ent.Internals[i].IsPrimaryKey)
                {
                    if (first) { first = false; } else { update += ", "; }
                    update += (ent.Internals[i].ColumnName + " = " + ("@x" + i.ToString()));

                    param = cmd.CreateParameter();
                    param.ParameterName = ("@x" + i.ToString());
                    param.Value = ent.Internals[i].ToColumnType(ent.Internals[i].GetValue(obj));

                    cmd.Parameters.Add(param);
                }
            }
            cmd.CommandText += (") VALUES (" + insert + ") " + update);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
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
                    if (i.GetType() != type) continue;

                    if (type._GetEntity().PrimaryKey.GetValue(i).Equals(primarykey)) { return i; }
                }
            }

            return null;
        }


        /// <summary>Creates an object from a database reader.</summary>
        /// <param name="type">Type.</param>
        /// <param name="reader">Reader.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>Object.</returns>
        internal static object _CreateObject(Type type, IDataReader reader, ICollection<object> localCache)
        {
            __Entity ent = type._GetEntity();
            object returnValue = _SearchCache(type, ent.PrimaryKey.ToFieldType(reader.GetValue(reader.GetOrdinal(ent.PrimaryKey.ColumnName)), localCache), localCache);

            if (returnValue == null)
            {
                if (localCache == null) { localCache = new List<object>(); }
                localCache.Add(returnValue = Activator.CreateInstance(type));
            }

            foreach (__Field internalField in ent.Internals)
            {
                internalField.SetValue(returnValue, internalField.ToFieldType(reader.GetValue(reader.GetOrdinal(internalField.ColumnName)), localCache));
            }

            foreach (__Field externalField in ent.Externals)
            {
                externalField.SetValue(returnValue, externalField.Fill(Activator.CreateInstance(externalField.Type), returnValue, localCache));
            }

            return returnValue;
        }


        /// <summary>Creates an instance by its primary keys.</summary>
        /// <param name="type">Type.</param>
        /// <param name="privateKey">Primary key.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>Object.</returns>
        internal static object _CreateObject(Type type, object privateKey, ICollection<object> localCache)
        {
            object returnValue = _SearchCache(type, privateKey, localCache);

            if (returnValue == null)
            {
                IDbCommand cmd = Connection.CreateCommand();

                cmd.CommandText = type._GetEntity().GetSQL() + " WHERE " + type._GetEntity().PrimaryKey.ColumnName + " = :pk";

                IDataParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = (":pk");
                parameter.Value = privateKey;
                cmd.Parameters.Add(parameter);

                IDataReader re = cmd.ExecuteReader();
                if (re.Read())
                {
                    returnValue = _CreateObject(type, re, localCache);
                }

                re.Close();
                cmd.Dispose();
            }

            if (returnValue == null) { throw new Exception("No data."); }
            return returnValue;
        }
    }
}
