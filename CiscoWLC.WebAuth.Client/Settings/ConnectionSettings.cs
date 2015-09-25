using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class ConnectionSettings
    {
        internal ConnectionSettings(ISharedPreferences sharedPrefs)
        {
            Ssid = sharedPrefs.GetString("pref_ssid", null);
        }

        public string Ssid { get; private set; }
    }
}