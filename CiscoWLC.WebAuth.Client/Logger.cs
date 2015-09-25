using System;

namespace CiscoWLC.WebAuth.Client
{
    public static class Logger
    {
        private static Action<string> _logAction;

        public static void Init(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public static void Error(string message)
        {
            _logAction(message);
        }
        public static void Warning(string message)
        {
            _logAction(message);
        }
        public static void Info(string message)
        {
            _logAction(message);
        }
    }
}