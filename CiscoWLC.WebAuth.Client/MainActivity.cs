using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace CiscoWLC.WebAuth.Client
{
    [Activity(Label = "CiscoWLC.WebAuth.Client", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _button;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _button = FindViewById<Button>(Resource.Id.button);
            _button.Click += OnButtonClick;
        }

        
        protected override void OnStart()
        {
            Logger.Init(Console.WriteLine);
            Logger.Info("Starting");
            base.OnStart();
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            Logger.Info("Click");
            Toast.MakeText(this, "Click", ToastLength.Short).Show();
        }

    }
}

