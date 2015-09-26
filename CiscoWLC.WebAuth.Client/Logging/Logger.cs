using System;

namespace CiscoWLC.WebAuth.Client.Logging
{
    public static class Logger
    {
        private static Action<Severity, string> _logAction;

        public static void Init(Action<Severity, string> logAction)
        {
            _logAction = logAction;
        }

        public static void Error(string message)
        {
            Log(Severity.Error, message);
        }

        public static void Warn(string message)
        {
            Log(Severity.Warn, message);
        }

        public static void Info(string message)
        {
            Log(Severity.Info, message);
        }

        public static void Verbose(string message)
        {
            Log(Severity.Verbose, message);
        }

        public static void Debug(string message)
        {
            Log(Severity.Debug, message);
        }

        private static void Log(Severity severity, string message)
        {
            if (_logAction != null)
                _logAction(severity, message);
            else
                Console.WriteLine($"{severity}: {message}");
        }
    }
}