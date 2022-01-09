using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework.Interfaces
{
    public interface ILocking
    {
        /// <summary>
        /// Locks an object.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <exception cref="LockingException">Thrown when the object could not be locked.</exception>
        /// <exception cref="ObjectLockedException">Thrown when the object is already locked by another instance.</exception>
        void Lock(object obj);

        /// <summary>
        /// Releases a lock on an object.
        /// </summary>
        /// <param name="obj">Object.</param>
        void Unlock(object obj);

        /// <summary>
        /// Releases all locks on objects
        /// </summary>
        void Purge();
    }
}
