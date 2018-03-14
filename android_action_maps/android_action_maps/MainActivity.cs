using Android.App;
using Android.Widget;
using Android.OS;

namespace android_action_maps
{
    [Activity(Label = "android_action_maps", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}

