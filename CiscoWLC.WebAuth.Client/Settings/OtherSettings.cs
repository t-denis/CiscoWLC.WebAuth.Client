using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class OtherSettings
    {
        internal OtherSettings(ISharedPreferences sharedPrefs)
        {
            ConnectTimeout = int.Parse(sharedPrefs.GetString("pref_timeout_connect", "0"));
            ShowVerboseLoggingToasts = sharedPrefs.GetBoolean("pref_show_verbose_toasts", false);
            IgnoreSslCertErrors = sharedPrefs.GetBoolean("pref_ignore_ssl_errors", true);
        }

        public bool IgnoreSslCertErrors { get; private set; }
        public int ConnectTimeout { get; private set; }
        public bool ShowVerboseLoggingToasts { get; private set; }
    }
}