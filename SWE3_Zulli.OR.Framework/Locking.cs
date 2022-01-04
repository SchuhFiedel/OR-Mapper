using SWE3_Zulli.OR.Framework.Exceptions;
using SWE3_Zulli.OR.Framework.Interfaces;
using SWE3_Zulli.OR.Framework.MetaModel;
using System;
using System.Data;

namespace SWE3_Zulli.OR.Framework
{
    public class Locking : ILocking
    {
        /// <summary>Gets this session's key.</summary>
        public string SessionKey
        {
            get; private set;
        }

        /// <summary>Creates a new instance of this class.</summary>
        public Locking()
        {
            SessionKey = Guid.NewGuid().ToString();

            try
            {
                using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE LOCKS (JCLASS VARCHAR(48) NOT NULL, JOBJECT VARCHAR(48) NOT NULL, JTIME TIMESTAMP NOT NULL, JOWNER VARCHAR(48) NOT NULL)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex ) 
            {
                BasicException.WriteException(ex);
            }
            try
            {
                using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
                {
                    cmd.CommandText = "CREATE UNIQUE INDEX UX_LOCKS ON LOCKS(JCLASS, JOBJECT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                BasicException.WriteException(ex);
            }
        }


        /// <summary>Locks an object.</summary>
        /// <param name="obj">Object.</param>
        /// <exception cref="LockingException">Thrown when the object could not be locked.</exception>
        /// <exception cref="ObjectLockedException">Thrown when the object is already locked by another instance.</exception>
        public virtual void Lock(object obj)
        {
            string owner = _GetLock(obj);

            if (owner == SessionKey) return;
            if (owner == null)
            {
                _CreateLock(obj);
                owner = _GetLock(obj);
            }

            if (owner != SessionKey) 
            { 
                throw new ObjectLockedEx(); 
            }
        }


        /// <summary>Releases a lock on an object.</summary>
        /// <param name="obj">Object.</param>
        public void Unlock(object obj)
        { 
            var keys = _GetKeys(obj);

            using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM LOCKS WHERE JCLASS = @c AND JOBJECT = @o AND JOWNER = @s";

                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = "@c";
                p.Value = keys.ClassKey;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@o";
                p.Value = keys.ObjectKey;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@s";
                p.Value = SessionKey;
                cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            Console.WriteLine($"Successfully Unlocked : {obj.ToString()}");
        }

        /// <summary>Purges timed out locks.</summary>
        public void Purge()
        {
            using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM LOCKS";// > @t";
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            Console.WriteLine("Delete All Locks");
        }

        /// <summary>Gets class and object key for an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns a tuple containing class and object key.</returns>
        private (string ClassKey, string ObjectKey) _GetKeys(object obj)
        {
            Table ent = obj._GetTable();
            return (ent.TableName, ent.PrimaryKey.ToColumnType(ent.PrimaryKey.GetValue(obj)).ToString());
        }

        /// <summary>Gets the current lock owner for an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Owner key.</returns>
        private string _GetLock(object obj)
        {
            var keys = _GetKeys(obj);
            string rval = null;

            using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT JOWNER FROM LOCKS WHERE JCLASS = :c AND JOBJECT = @o";

                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = "@c";
                p.Value = keys.ClassKey;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@o";
                p.Value = keys.ObjectKey;
                cmd.Parameters.Add(p);
                using (IDataReader re = cmd.ExecuteReader())
                {
                    if (re.Read())
                    {
                        rval = re.GetString(0);
                    }
                    re.Close();
                    re.Dispose();
                }
                cmd.Dispose();
            }
            return rval;
        }

        /// <summary>Creates a lock on an object.</summary>
        /// <param name="obj">Object.</param>
        private void _CreateLock(object obj)
        {
            var keys = _GetKeys(obj);

            using (IDbCommand cmd = ORMapper.Connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO LOCKS(JCLASS, JOBJECT, JTIME, JOWNER) VALUES (@c, @o, Current_Timestamp, @s)";

                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = "@c";
                p.Value = keys.ClassKey;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@o";
                p.Value = keys.ObjectKey;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@s";
                p.Value = SessionKey;
                cmd.Parameters.Add(p);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception) { }

                cmd.Dispose();
            }
        }
    }
}
