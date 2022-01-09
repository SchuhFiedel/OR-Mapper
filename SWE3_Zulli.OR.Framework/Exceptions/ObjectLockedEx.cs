using System;

namespace SWE3_Zulli.OR.Framework.Exceptions
{
    /// <summary>
    /// Exception that should be used when an object is in use by another DB Connection
    /// </summary>
    public class  ObjectLockedEx: Exception
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public ObjectLockedEx() : base("Object locked by another session.")
        { }
    }
}
