using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class OtherSettings
    {
        internal OtherSettings(ISharedPreferences sharedPrefs)
        {
            ConnectTimeout = int.Parse(sharedPrefs.GetString("pref_timeout_connect", "10000"));
            ConnectCheckInterval = int.Parse(sharedPrefs.GetString("pref_interval_check_connect", "200"));
            ShowVerboseLoggingToasts = sharedPrefs.GetBoolean("pref_show_verbose_toasts", false);
            IgnoreSslCertErrors = sharedPrefs.GetBoolean("pref_ignore_ssl_errors", true);
        }

        public bool IgnoreSslCertErrors { get; private set; }
        public int ConnectTimeout { get; private set; }
        public int ConnectCheckInterval { get; set; }
        public bool ShowVerboseLoggingToasts { get; private set; }
    }
}