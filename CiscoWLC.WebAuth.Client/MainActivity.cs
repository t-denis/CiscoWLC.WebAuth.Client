using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using CiscoWLC.WebAuth.Client.Core;
using CiscoWLC.WebAuth.Client.Logging;
using CiscoWLC.WebAuth.Client.Settings;

namespace CiscoWLC.WebAuth.Client
{
    [Activity(Label = "CiscoWLC.WebAuth.Client", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private Button _button;
        private bool _isBusy;

        private MainSettings _settings;

        public MainSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = MainSettings.GetCurrent(this);
                return _settings;
            }
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Logger.Init(LogMessage);
            Logger.Verbose("Creating MainActivity");

            _button = FindViewById<Button>(Resource.Id.button);
            _button.Click += OnButtonClick;
            PreferenceManager.GetDefaultSharedPreferences(this).RegisterOnSharedPreferenceChangeListener(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PreferenceManager.GetDefaultSharedPreferences(this).UnregisterOnSharedPreferenceChangeListener(this);
        }

        protected override void OnStart()
        {
            UpdateActivityState();
            Logger.Verbose("Starting MainActivity");
            base.OnStart();
        }

        private void LogMessage(Severity severity, string message)
        {
            Console.WriteLine($"{severity}: {message}");

            switch (severity)
            {
                case Severity.Debug:
                    break;
                case Severity.Verbose:
                    if (Settings.OtherSettings.ShowVerboseLoggingToasts)
                        ShowToast(message);
                    break;
                case Severity.Info:
                case Severity.Warn:
                case Severity.Error:
                    ShowToast(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            if (_button.Text == GetString(Resource.String.Settings))
                StartActivity(typeof(SettingsActivity));
            else
            {
                if (_isBusy)
                {
                    ShowToast(GetString(Resource.String.Busy));
                    return;
                }
                try
                {
                    _isBusy = true;
                    _button.Text = GetString(Resource.String.Busy);
                    Logger.Verbose("Connecting");
                    var settings = Settings;
                    var conManager = new WifiConnector();
                    conManager.Connect(this, new Ssid(settings.ConnectionSettings.Ssid), settings.OtherSettings);

                    var webAuthManager = new CiscoWebAuthManager();
                    await webAuthManager.LoginAsync(settings.LoginPageSettings, settings.AuthSettings);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
                finally
                {
                    _isBusy = false;
                    UpdateActivityState();
                }
            }
        }

        #region Menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.settings:
                    StartActivity(typeof(SettingsActivity));
                    return true;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        #endregion

        private void ShowToast(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        private void UpdateActivityState()
        {
            if (Settings.OtherSettings.IgnoreSslCertErrors)
                ServicePointManager.ServerCertificateValidationCallback = AllowInvalidSslCertificates;
            else
                ServicePointManager.ServerCertificateValidationCallback = null;

            _button.Text = GetString(Settings.AreValid()
                ? Resource.String.Connect
                : Resource.String.Settings);
        }

        private bool AllowInvalidSslCertificates(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Logger.Verbose("Accepting SSL Certificate");
            return true;
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            _settings = MainSettings.GetCurrent(this);
            UpdateActivityState();
        }
    }
}

