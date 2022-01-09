using System;

namespace SWE3_Zulli.OR.Framework.Exceptions
{
    /// <summary>
    /// A class to easily print exceptions in a disctinct color in the console
    /// </summary>
    public static class BasicException
    {
        /// <summary>
        /// Function to easily print exceptions in a red color in the console
        /// </summary>
        public static void WriteException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}
