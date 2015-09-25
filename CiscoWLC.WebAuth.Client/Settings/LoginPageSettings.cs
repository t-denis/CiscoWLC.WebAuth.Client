using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class LoginPageSettings
    {
        internal LoginPageSettings(ISharedPreferences sharedPrefs)
        {
            LoginPageUrl = sharedPrefs.GetString("pref_loginpage", null);
            IgnoreSslCertErrors = sharedPrefs.GetBoolean("pref_ignore_ssl_errors", true);
        }

        public string LoginPageUrl { get; private set; }
        public bool IgnoreSslCertErrors { get; private set; }
    }
}