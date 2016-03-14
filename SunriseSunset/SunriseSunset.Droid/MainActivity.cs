using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using OxyPlot.Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android;

namespace SunriseSunset.Droid
{
    [Activity(Label = "SunriseSunset", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            PlotViewRenderer.Init();

            ToolbarResource = Resource.Layout.toolbar;

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

