using System;

namespace SWE3_Zulli.OR.Framework.Exceptions
{
    public static class BasicException
    {
        public static void WriteException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}
