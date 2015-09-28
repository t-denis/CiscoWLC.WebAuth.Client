using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class OtherSettings
    {
        internal OtherSettings(ISharedPreferences sharedPrefs)
        {
            ConnectTimeout = int.Parse(sharedPrefs.GetString("pref_timeout_connect", "10000"));
            ConnectCheckInterval = int.Parse(sharedPrefs.GetString("pref_interval_check_connect", "200"));
            ToastLevel = (ToastLevel)int.Parse(sharedPrefs.GetString("pref_toasts_level", "2"));
            IgnoreSslCertErrors = sharedPrefs.GetBoolean("pref_ignore_ssl_errors", true);
            StartUrl = sharedPrefs.GetString("pref_start_url", null);
        }

        public bool IgnoreSslCertErrors { get; private set; }
        public int ConnectTimeout { get; private set; }
        public int ConnectCheckInterval { get; private set; }
        public ToastLevel ToastLevel { get; private set; }
        public string StartUrl { get; private set; }
    }

    public enum ToastLevel
    {
        None = 1,
        WarningsErrorsAndConnected = 2,
        Info = 3,
        Verbose = 4
    }
}