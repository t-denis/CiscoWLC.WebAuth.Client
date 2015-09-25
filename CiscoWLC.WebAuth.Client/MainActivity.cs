using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CiscoWLC.WebAuth.Client.Settings;

namespace CiscoWLC.WebAuth.Client
{
    [Activity(Label = "CiscoWLC.WebAuth.Client", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const int SettingsActivityId = 1;
        private Button _button;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _button = FindViewById<Button>(Resource.Id.button);
            _button.Click += OnButtonClick;
            UpdateButtonText();
        }
        
        protected override void OnStart()
        {
            Logger.Init(Console.WriteLine);
            Logger.Info("Starting");
            UpdateButtonText();
            base.OnStart();
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            StartActivityForResult(typeof(SettingsActivity), 1);
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == SettingsActivityId)
            {
                UpdateButtonText();
            }
        }

        #endregion

        private void UpdateButtonText()
        {
            var settings = MainSettings.GetCurrent(this);
            if (settings.AreValid())
            {
                _button.Text = "Connect";
            }
            else
            {
                _button.Text = "Settings";
            }
        }
    }
}

