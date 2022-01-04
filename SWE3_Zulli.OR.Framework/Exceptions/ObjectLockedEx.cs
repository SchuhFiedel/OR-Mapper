using System;

namespace SWE3_Zulli.OR.Framework.Exceptions
{
    public class  ObjectLockedEx: Exception
    {
        /// <summary>Creates a new instance of this class.</summary>
        public ObjectLockedEx() : base("Object locked by another session.")
        { }
    }
}
