using Android.App;
using Android.Widget;
using Android.OS;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System.Net;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using Tweetinvi;
using Tweetinvi.Models;
using Android.Speech.Tts;
using Android.Runtime;

namespace android_action_maps
{
    [Activity(Label = "Action Maps", MainLauncher = true, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity, TextToSpeech.IOnInitListener
    {
        public SceneView MySceneView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            MySceneView = FindViewById<SceneView>(Resource.Id.MySceneView);

            /* set the scene up */
            agSetupScene();

            /* add building */
            agAddBuilding();

            /* Demo Dots */
            //agAddDemoPoints();

            /* Start twitter stream */
            twitter_hose();

            /* Grab the plane data */
            plane_data();

            /* Grab the bikeshare data */
            bike_data();

            /* Grab the metro data */
            metro_data();
        }


        #region metro

        public class MetroInnerJSON
        {
            public double longitude;
            public double latitude;
        }

        public class MetroJSON
        {
            public MetroInnerJSON[] items;

        }
        GraphicsOverlay metro;
        public void metro_data()
        {
            /* first get stops */
            
            Task.Factory.StartNew(async () =>
            {
                Uri u = new System.Uri("https://services1.arcgis.com/gOY38HDnFUYPDony/ArcGIS/rest/services/la_busstops1217/FeatureServer/1");

                var agsl_stop = new ServiceFeatureTable(u);
                await agsl_stop.LoadAsync();
                this.RunOnUiThread(
                    () =>
                    {
                        var stopsLayer = new Esri.ArcGISRuntime.Mapping.FeatureLayer(agsl_stop);
                        MySceneView.Scene.OperationalLayers.Add(stopsLayer);
                    });
            });
            

            /* then get vehicles */

            WebClient client = new WebClient();
            var wgs84 = MySceneView.Scene.SpatialReference;

            metro = new GraphicsOverlay();
            
            var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.IndianRed, 8);
            metro.Renderer = new SimpleRenderer(pmark);
            metro.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            Task.Factory.StartNew(async () => {
                while (true)
                {

                    string downloadString = client.DownloadString("http://api.metro.net/agencies/lametro/vehicles/");
                    MetroJSON res = JsonConvert.DeserializeObject<MetroJSON>(downloadString);
                    metro.Graphics.Clear();
                    foreach (MetroInnerJSON vehicle in res.items)
                    {
                        var ploc = new MapPoint(vehicle.longitude, vehicle.latitude, 0, wgs84);
                        var ptest = new Graphic(ploc);
                        metro.Graphics.Add(ptest);
                    }

                    await Task.Delay(2300);

                }
            });

            MySceneView.GraphicsOverlays.Add(metro);
        }
        #endregion
        #region planes

        public class AirplaneInnerJSON
        {
            public double Long;
            public double Lat;
            public double Alt;
        }

        public class AirplaneJSON
        {            
            public AirplaneInnerJSON[] acList;

        }

        GraphicsOverlay planes;
        public void plane_data()
        {

            WebClient client = new WebClient();
            var wgs84 = MySceneView.Scene.SpatialReference;

            planes = new GraphicsOverlay();
            planes.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;

            var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 10f);
            planes.Renderer = new SimpleRenderer(pmark);
            

            Task.Factory.StartNew(async () => {
                while (true)
                {

                    string downloadString = client.DownloadString("http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat=34&lng=-118.28&fDstL=0&fDstU=100");
                    AirplaneJSON res = JsonConvert.DeserializeObject<AirplaneJSON>(downloadString);
                    planes.Graphics.Clear();
                    foreach (AirplaneInnerJSON plane in res.acList)
                    {
                        var ploc = new MapPoint(plane.Long, plane.Lat, plane.Alt, wgs84);
                        var ptest = new Graphic(ploc);
                        planes.Graphics.Add(ptest);
                    }

                    await Task.Delay(1250);

                }
            });

            MySceneView.GraphicsOverlays.Add(planes);


        }
        #endregion
        #region bikes
        public class BikePropJSON
        {
            public int bikesAvailable;
            public int totalDocks;
            public double longitude;
            public double latitude;
        }
        public class BikeInnerJSON
        {
            public BikePropJSON properties;
        }

        public class BikeJSON
        {
            public BikeInnerJSON[] features;
        }
        GraphicsOverlay bikes;
        public void bike_data()
        {

            WebClient client = new WebClient();
            var wgs84 = MySceneView.Scene.SpatialReference;

            bikes = new GraphicsOverlay();
            bikes.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            Task.Factory.StartNew(async () => {
                while (true)
                {

                    string downloadString = client.DownloadString("https://bikeshare.metro.net/stations/json/");
                    BikeJSON res = JsonConvert.DeserializeObject<BikeJSON>(downloadString);
                    bikes.Graphics.Clear();
                    foreach (BikeInnerJSON bike in res.features)
                    {
                        var ploc = new MapPoint(bike.properties.longitude, bike.properties.latitude, 0, wgs84);
                        float full = (bike.properties.bikesAvailable + 0.1f) / (bike.properties.totalDocks + 0.1f);
                        var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.FromArgb(255, 0, (byte)(255 * full), 0), 10);
                        var ptest = new Graphic(ploc, pmark);
                        bikes.Graphics.Add(ptest);
                    }

                    await Task.Delay(25000);

                }
            });

            MySceneView.GraphicsOverlays.Add(bikes);
        }
        #endregion


        #region twitter
        
      
        public void twitter_hose()
        {

            #region SECRETS

            /***
             *  DONT COMMIT ME
             */
            var creds = Auth.SetUserCredentials("", "", "", "");
            /***
             *  DONT COMMIT ME
             */
            #endregion


            // SpeechSynthesizer synth = new SpeechSynthesizer();
            TextToSpeech ttobj = new TextToSpeech(Application.Context, this);
           
            /* set up twitter stuff */
            var stream = Stream.CreateFilteredStream();
            var top_left = new Coordinates(34.035199, -118.309177);
            var bottom_right = new Coordinates(33.996693, -118.2616002);

            stream.AddLocation(top_left, bottom_right);
            stream.MatchingTweetReceived += (sender, args) =>
            {
                
                /* speech synth */
                ttobj.Speak(args.Tweet.CreatedBy.ScreenName + ": " + args.Tweet.Text, QueueMode.Flush, null, null);
            };

            Task.Factory.StartNew(async () =>
            {
                stream.StartStreamMatchingAllConditions();
            });
        }
        #endregion

        ArcGISSceneLayer agsl;
        public void agAddBuilding()
        {
            Task.Factory.StartNew(async () =>
            {
                Uri u = new System.Uri("/mnt/sdcard/LARIAC_BUILDINGS_2014.slpk", UriKind.Relative);

                agsl = new ArcGISSceneLayer(u);
                await agsl.LoadAsync();
                this.RunOnUiThread(
                    () =>
                    {
                        MySceneView.Scene.OperationalLayers.Add(agsl);
                    });

            });

        }
        public void agSetupScene()
        {
            MySceneView.Scene = new Scene(Basemap.CreateLightGrayCanvas());
            
            MySceneView.Scene.BaseSurface.ElevationSources.Add(new ArcGISTiledElevationSource(new System.Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer")));
            
            MySceneView.StereoRendering = new SideBySideBarrelDistortionStereoRendering();
            MySceneView.IsAttributionTextVisible = false;

            // USC
            var camera = new Camera(34.02209, -118.2853, 300, 0, 0, 0);
            MySceneView.SetViewpointCamera(camera);

            var fpcController = new FirstPersonCameraController(camera);
  
            var phoneSensors = new PhoneMotionDataSource();
            fpcController.DeviceMotionDataSource = phoneSensors;
            fpcController.Framerate = FirstPersonFrameRate.Speed; 
            MySceneView.CameraController = fpcController;
            phoneSensors.StartUpdatingAngles(false);

        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
           // throw new NotImplementedException();
        }
    }

}

