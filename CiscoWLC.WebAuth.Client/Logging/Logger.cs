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
            _logAction(Severity.Error, message);
        }

        public static void Warn(string message)
        {
            _logAction(Severity.Warn, message);
        }

        public static void Info(string message)
        {
            _logAction(Severity.Info, message);
        }

        public static void Verbose(string message)
        {
            _logAction(Severity.Verbose, message);
        }

        public static void Debug(string message)
        {
            _logAction(Severity.Debug, message);
        }
    }
}