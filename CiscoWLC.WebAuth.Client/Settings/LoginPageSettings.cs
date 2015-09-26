using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class LoginPageSettings
    {
        internal LoginPageSettings(ISharedPreferences sharedPrefs)
        {
            LoginPageUrl = sharedPrefs.GetString("pref_loginpage", null);
        }

        public string LoginPageUrl { get; private set; }
    }
}