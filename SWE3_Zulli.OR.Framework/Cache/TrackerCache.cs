using SWE3_Zulli.OR.Framework.Interfaces;
using SWE3_Zulli.OR.Framework.MetaModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework.Cache
{
    public class TrackerCache: BasicCache, ICache
    {
        /// <summary>
        /// Hash items.
        /// </summary>
        protected Dictionary<Type, Dictionary<object, string>> _Hashes = new Dictionary<Type, Dictionary<object, string>>();

        /// <summary>
        /// Gets the hash for a type.
        /// </summary>
        /// <param name="t">Type.</param>
        /// <returns>Type hash store.</returns>
        protected virtual Dictionary<object, string> _GetHash(Type t)
        {
            if (_Hashes.ContainsKey(t)) 
            { 
                return _Hashes[t]; 
            }

            Dictionary<object, string> rval = new Dictionary<object, string>();
            _Hashes.Add(t, rval);

            return rval;
        }

        /// <summary>
        /// Gets the hash for an object.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>Hash</returns>
        protected string _ComputeHash(object obj)
        {
            string rval = "";
            foreach (Column i in obj._GetTable().InternalFields)
            {
                if (i.IsForeignKey)
                {
                    object m = i.GetValue(obj);
                    if (m != null) 
                    { 
                        rval += m._GetTable()
                            .PrimaryKey
                            .GetValue(m)
                            .ToString(); 
                    }
                }
                else { 
                    rval += (i.ColumnName + "=" + i.GetValue(obj).ToString() + ";"); 
                }
            }

            foreach (Column i in obj._GetTable().ExternalFields)
            {
                IEnumerable m = (IEnumerable)i.GetValue(obj);

                if (m != null)
                {
                    rval += (i.ColumnName + "=");
                    foreach (object k in m)
                    {
                        rval += k._GetTable()
                            .PrimaryKey
                            .GetValue(k)
                            .ToString()
                            + ",";
                    }
                }
            }

            return Encoding.UTF8.GetString(
                SHA256
                .Create()
                .ComputeHash(Encoding.UTF8.GetBytes(rval))
                );
        }

        /// <summary>
        /// Adds an object to the cache.
        /// </summary>
        /// <param name="obj">Object.</param>
        public override void PutObject(object obj)
        {
            base.PutObject(obj);
            if (obj != null) 
            {
                _GetHash(obj.GetType())[obj._GetTable()
                .PrimaryKey.GetValue(obj)]
                = _ComputeHash(obj); 
            }
        }


        /// <summary>
        /// Removes an object from the cache.
        /// </summary>
        /// <param name="obj">Object.</param>
        public override void RemoveObject(object obj)
        {
            base.RemoveObject(obj);
            _GetHash(
                obj.GetType()
            )
            .Remove(
                obj._GetTable()
                .PrimaryKey
                .GetValue(obj)
            );
        }

        /// <summary>
        /// Checks if an object has changes
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>
        /// Returns TRUE if the object has changed or might have changed, returns FALSE if the object is unchanged.
        /// </returns>
        public override bool ObjectHasChanged(object obj)
        {
            Dictionary<object, string> h = _GetHash(obj.GetType());
            object pk = obj._GetTable().PrimaryKey.GetValue(obj);

            if (h.ContainsKey(pk))
            {
                return (h[pk] == _ComputeHash(obj));
            }

            return true;
        }
    }
}
