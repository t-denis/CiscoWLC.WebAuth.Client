using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
        private Button _button;
        private bool _isBusy;

        private MainSettings _settings;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _settings = MainSettings.GetCurrent(this);

            Logger.Init(LogMessage);
            Logger.Verbose("Creating MainActivity");
            
            _button = FindViewById<Button>(Resource.Id.button);
            _button.Click += OnButtonClick;
        }

        protected override void OnStart()
        {
            _settings = MainSettings.GetCurrent(this);
            UpdateActivityState();
            Logger.Verbose("Starting MainActivity");
            base.OnStart();

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
                    var settings = _settings;
                    var conManager = new WifiConnector();
                    var ssid = new Ssid(settings.ConnectionSettings.Ssid);
                    await conManager.ConnectAsync(this, ssid,
                        TimeSpan.FromMilliseconds(settings.OtherSettings.ConnectCheckInterval),
                        TimeSpan.FromMilliseconds(settings.OtherSettings.ConnectTimeout));
                    
                    var webAuthManager = new CiscoWebAuthManager();
                    await webAuthManager.LoginAsync(settings.LoginPageSettings, settings.AuthSettings);
                    if (settings.OtherSettings.ToastLevel == ToastLevel.WarningsErrorsAndConnected)
                        ShowToast("Connected");

                    OpenWebBrowserIfRequired(settings);
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

        #region Logging

        private void LogMessage(Severity severity, string message)
        {
            Console.WriteLine($"{severity}: {message}");

            var allowedToastLevels = new ToastLevel[] { };

            var toastLevel = _settings.OtherSettings.ToastLevel;
            switch (severity)
            {
                case Severity.Debug:
                    break;
                case Severity.Verbose:
                    allowedToastLevels = new[] { ToastLevel.Verbose };
                    break;
                case Severity.Info:
                    allowedToastLevels = new[] { ToastLevel.Info, ToastLevel.Verbose };
                    break;
                case Severity.Warn:
                case Severity.Error:
                    allowedToastLevels = new[]
                    {ToastLevel.Info, ToastLevel.Verbose, ToastLevel.WarningsErrorsAndConnected};
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
            if (allowedToastLevels.Contains(toastLevel))
                ShowToast(message);
        }

        #endregion
        
        #region WebBrowser

        private void OpenWebBrowserIfRequired(MainSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.OtherSettings.StartUrl))
            {
                var uri = Android.Net.Uri.Parse(settings.OtherSettings.StartUrl);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
        }

        #endregion

        #region Toasts

        private void ShowToast(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        #endregion

        #region SSL

        private bool AllowInvalidSslCertificates(object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            Logger.Verbose("Accepting SSL Certificate");
            return true;
        }

        #endregion

        private void UpdateActivityState()
        {
            if (_settings.OtherSettings.IgnoreSslCertErrors)
                ServicePointManager.ServerCertificateValidationCallback = AllowInvalidSslCertificates;
            else
                ServicePointManager.ServerCertificateValidationCallback = null;

            _button.Text = GetString(_settings.AreValid()
                ? Resource.String.Connect
                : Resource.String.Settings);
        }

    }
}

