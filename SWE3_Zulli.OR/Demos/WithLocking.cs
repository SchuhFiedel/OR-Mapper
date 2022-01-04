using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Framework.Exceptions;
using SWE3_Zulli.OR.Models;
using System;

namespace SWE3_Zulli.OR.Demos
{
    public static class WithLocking
    { 
        public static void Show()
        {
            Console.WriteLine("(8) Locking demonstration");
            Console.WriteLine("-------------------------");
            Console.WriteLine();
            
            
            ORMapper.Lock = new Locking();
            ORMapper.Purge();
            ORMapper.Lock = new Locking();
            Teacher t = ORMapper.Get<Teacher>(01);
            ORMapper.LockObject(t);

            ORMapper.Lock = new Locking();
            try
            {
                ORMapper.LockObject(t);
            }
            catch (Exception ex) 
            {
                BasicException.WriteException(ex);
            }
            try
            {
                ORMapper.Unlock(t);
            }
            catch(Exception ex)
            {
                BasicException.WriteException(ex);
            }

            ORMapper.Purge();
        }
    }
}
