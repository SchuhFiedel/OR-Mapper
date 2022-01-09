using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWE3_Zulli.OR.Framework.Interfaces;

namespace SWE3_Zulli.OR.Framework.Cache
{
    public class BasicCache : ICache
    {

        /// <summary>
        /// Cache Dictionary.
        /// </summary>
        protected Dictionary<Type, Dictionary<object, object>> _Caches = new Dictionary<Type, Dictionary<object, object>>();

        /// <summary>
        /// Gets the cache for a type.
        /// </summary>
        /// <param name="t">Type.</param>
        /// <returns>Type cache.</returns>
        protected virtual Dictionary<object, object> _GetCache(Type t)
        {
            if (_Caches.ContainsKey(t)) { return _Caches[t]; }

            Dictionary<object, object> rval = new Dictionary<object, object>();
            _Caches.Add(t, rval);

            return rval;
        }

        /// <summary>
        /// Returns if the cache contains an object with the given primary key and type.
        /// </summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>
        /// Returns TRUE if the object is in the Cache, otherwise returns FALSE.
        /// </returns>
        public virtual bool ContainsObject(Type t, object pk)
        {
            return _GetCache(t)
                .ContainsKey(pk);
        }

        /// <summary>
        /// Returns if the cache contains an object with the given primary key.
        /// </summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>
        /// Returns TRUE if the object is in the Cache, otherwise returns FALSE.
        /// </returns>
        public virtual bool ContainsObject(object obj)
        {
            return _GetCache(
                    obj.GetType()
                )
                .ContainsKey(
                    obj._GetTable()
                    .PrimaryKey
                    .GetValue(obj)
                );
        }

        /// <summary>
        /// Gets an object from the cache.
        /// </summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Object</returns>
        public virtual object GetObject(Type t, object pk)
        {
            Dictionary<object, object> c = _GetCache(t);

            if (c.ContainsKey(pk)) 
            { 
                return c[pk]; 
            }
            return null;
        }

        /// <summary>
        /// Check if an Object has Changed
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>
        /// Returns TRUE if the object has changed or might have changed, returns FALSE if the object is unchanged.
        /// </returns>
        public virtual bool ObjectHasChanged(object obj)
        {
            return true;
        }

        /// <summary>
        /// Adds an Object to the Cache
        /// </summary>
        /// <param name="obj">Object.</param>
        public virtual void PutObject(object obj)
        {
            if (obj != null) { _GetCache(obj.GetType())[obj._GetTable().PrimaryKey.GetValue(obj)] = obj; }
        }

        /// <summary>
        /// Removes an object from the cache.
        /// </summary>
        /// <param name="obj">Object.</param>
        public virtual void RemoveObject(object obj)
        {
            _GetCache(
                    obj.GetType()
                )
                .Remove(
                    obj._GetTable()
                    .PrimaryKey
                    .GetValue(obj)
                );
        }

        /// <summary>
        /// Returns the Cache List
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type, Dictionary<object, object>> GetCacheList()
        {
            return _Caches;
        }
    }
}
