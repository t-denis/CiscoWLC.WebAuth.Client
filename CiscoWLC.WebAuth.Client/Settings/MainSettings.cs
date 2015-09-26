using Android.Content;
using Android.Preferences;

namespace CiscoWLC.WebAuth.Client.Settings
{
    public class MainSettings
    {
        private MainSettings() { }

        public ConnectionSettings ConnectionSettings { get; private set; }
        public LoginPageSettings LoginPageSettings { get; private set; }
        public AuthSettings AuthSettings { get; private set; }
        public OtherSettings OtherSettings { get; private set; }

        public static MainSettings GetCurrent(Context context)
        {
            var sharedPrefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var settings = new MainSettings
            {
                AuthSettings = new AuthSettings(sharedPrefs),
                ConnectionSettings = new ConnectionSettings(sharedPrefs),
                LoginPageSettings = new LoginPageSettings(sharedPrefs),
                OtherSettings = new OtherSettings(sharedPrefs)
            };
            return settings;
        }
    }
}