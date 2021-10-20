using SWE3_Zulli.OR.Framework.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_Zulli.OR.Framework
{
    class LazyObject<T>: ILazy
    {
        protected object _PrimaryKey;
        protected T _Value;
        protected bool _InitializedFlag = false;

        public LazyObject(object pk = null)
        {
            _PrimaryKey = pk;
        }

        public T Value {
            get { if (!_InitializedFlag) ORMapper.Get<T>(_PrimaryKey); _InitializedFlag = true; return _Value; } 
            set { _Value = value; _InitializedFlag = true; } 
        }

        public static implicit operator T(LazyObject<T> lazy)
        {
            return lazy.Value;
        }

        public static implicit operator LazyObject<T>(T obj)
        {
            LazyObject<T> retrunValue = new();
            return retrunValue;
        }
    }
}
