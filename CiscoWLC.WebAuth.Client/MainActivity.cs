using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CiscoWLC.WebAuth.Client.Core;
using CiscoWLC.WebAuth.Client.Logging;
using CiscoWLC.WebAuth.Client.Settings;

namespace CiscoWLC.WebAuth.Client
{
    [Activity(Label = "CiscoWLC.WebAuth.Client", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const int SettingsActivityId = 1;
        private Button _button;
        private bool _isBusy;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Logger.Init(LogMessage);
            Logger.Verbose("Creating MainActivity");

            _button = FindViewById<Button>(Resource.Id.button);
            _button.Click += OnButtonClick;
            UpdateButtonText();
        }

        protected override void OnStart()
        {
            Logger.Init(LogMessage);
            Logger.Verbose("Starting MainActivity");
            UpdateButtonText();
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
                    // TODO: Cache logging settings
                    var settings = MainSettings.GetCurrent(this);
                    if (settings.OtherSettings.ShowVerboseLoggingToasts)
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
                StartActivityForResult(typeof (SettingsActivity), 1);
            else
            {
                if (_isBusy)
                {
                    Logger.Info("Busy...");
                    return;
                }
                try
                {
                    _isBusy = true;

                    var settings = MainSettings.GetCurrent(this);
                    var conManager = new WifiConnector();
                    var networkInfo = new NetworkInfo(settings.ConnectionSettings);
                    conManager.Connect(this, networkInfo, settings.OtherSettings);

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
                    StartActivityForResult(typeof (SettingsActivity), SettingsActivityId);
                    return true;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        #endregion

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == SettingsActivityId)
            {
                UpdateButtonText();
                Logger.Info("Settings updated");
            }
        }

        private void ShowToast(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        private void UpdateButtonText()
        {
            var settings = MainSettings.GetCurrent(this);
            if (settings.AreValid())
            {
                _button.Text = GetString(Resource.String.Connect);
            }
            else
            {
                _button.Text = GetString(Resource.String.Settings);
            }
        }
    }
}

