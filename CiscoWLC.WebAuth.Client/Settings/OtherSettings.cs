using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class OtherSettings
    {
        internal OtherSettings(ISharedPreferences sharedPrefs)
        {
            ScanTimeout = int.Parse(sharedPrefs.GetString("pref_timeout_scan", "0"));
            ConnectTimeout = int.Parse(sharedPrefs.GetString("pref_timeout_connect", "0"));
            RecreateNetworks = sharedPrefs.GetBoolean("pref_recreate_networks", false);
            ShowVerboseLoggingToasts = sharedPrefs.GetBoolean("pref_show_verbose_toasts", false);
            IgnoreSslCertErrors = sharedPrefs.GetBoolean("pref_ignore_ssl_errors", true);
            CreateNetworks = sharedPrefs.GetBoolean("pref_create_networks", false);
        }

        public bool CreateNetworks { get; private set; }
        public bool IgnoreSslCertErrors { get; private set; }
        public int ConnectTimeout { get; private set; }
        public int ScanTimeout { get; private set; }
        public bool RecreateNetworks { get; private set; }
        public bool ShowVerboseLoggingToasts { get; private set; }
    }
}