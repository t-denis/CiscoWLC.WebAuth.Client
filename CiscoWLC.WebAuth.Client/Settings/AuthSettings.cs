using Android.Content;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class AuthSettings
    {
        internal AuthSettings(ISharedPreferences sharedPrefs)
        {
            Username = sharedPrefs.GetString("pref_username", null);
            Password = sharedPrefs.GetString("pref_password", null);
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}