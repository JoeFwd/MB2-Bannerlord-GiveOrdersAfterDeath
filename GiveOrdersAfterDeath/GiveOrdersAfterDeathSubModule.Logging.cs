using System;
using System.Diagnostics;
using TaleWorlds.Engine;

namespace GiveOrdersAfterDeath
{
    public partial class GiveOrdersAfterDeathSubModule
    {
        [Conditional("TRACE")]
        public static void Error(Exception exception, string message = null)
        {
            if (message != null)
                Error(message);

            var st = new StackTrace(exception, true);
            var f = st.GetFrame(0);
            var exceptionMessage = $"{f.GetFileName()}:{f.GetFileLineNumber()}:{f.GetFileColumnNumber()}: {exception.GetType().Name}: {exception.Message}";
            
            MBDebug.ConsolePrint(exceptionMessage);
            MBDebug.ConsolePrint(exception.StackTrace);
            Debugger.Log(3, nameof(GiveOrdersAfterDeath), exceptionMessage + Environment.NewLine);
            Debugger.Log(3, nameof(GiveOrdersAfterDeath), exception.StackTrace + Environment.NewLine);
        }

        [Conditional("TRACE")]
        public static void Error(string message = null)
        {
            if (message == null)
                return;

            if (!message.EndsWith("\n"))
                message += Environment.NewLine;
            Debugger.Log(3, nameof(GiveOrdersAfterDeath), message);
            Console.Error.WriteLine($"{nameof(GiveOrdersAfterDeath)}: ERROR: {message}");
        }
        
        [Conditional("DEBUG")]
        public static void Print(string message) {
            Debugger.Log(0, nameof(GiveOrdersAfterDeath), message + Environment.NewLine);
            Console.Error.WriteLine($"{nameof(GiveOrdersAfterDeath)}: DEBUG: {message}");
        }
    }
}