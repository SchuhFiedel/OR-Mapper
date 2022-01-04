using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework.Interfaces
{
    public interface ICache
    {
        /// <summary>Gets an object from the cache.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Object</returns>
        object GetObject(Type t, object pk);


        /// <summary>Puts an object into the cache.</summary>
        /// <param name="obj">Object.</param>
        void PutObject(object obj);


        /// <summary>Removes an object from the cache.</summary>
        /// <param name="obj">Object.</param>
        void RemoveObject(object obj);


        /// <summary>Returns if the cache contains an object with the given primary key.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Returns TRUE if the object is in the Cache, otherwise returns FALSE.</returns>
        bool ContainsObject(Type t, object pk);


        /// <summary>Returns if the cache contains an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object is in the Cache, otherwise returns FALSE.</returns>
        bool ContainsObject(object obj);


        /// <summary>Gets if an object has changed.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object has changed or might have changed, returns FALSE if the object is unchanged.</returns>
        bool ObjectHasChanged(object obj);

        public Dictionary<Type, Dictionary<object, object>> GetCacheList();
    }
}