using Android.App;
using Android.OS;
using Android.Preferences;

namespace CiscoWLC.WebAuth.Client
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : PreferenceActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        }
    }
}