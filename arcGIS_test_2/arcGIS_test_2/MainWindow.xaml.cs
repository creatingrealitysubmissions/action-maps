using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.UI;
using System.Diagnostics;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Tweetinvi;
using Tweetinvi.Models;
using IrrKlang;
using System.Net;
using Newtonsoft.Json;
using System.Speech.Synthesis;


namespace arcGIS_test_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // USC is 34.02209° N, -118.2852° E
        // intercontinental is 34.0498° N, -118.2606° E
        // SD is -117.161087, 32.715736

        //bottom right:
        // la = 33.895849
        // lo = -118.220071
        // top left:
        // la = 34.100245
        // lo = -118.459463
        //



        ISoundEngine engine;

        public void agSetupScene()
        {
            MySceneView.Scene = new Scene(Basemap.CreateLightGrayCanvasVector());
            MySceneView.Scene.BaseSurface.ElevationSources.Add(new ArcGISTiledElevationSource(new System.Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer")));
            //MySceneView.StereoRendering = new SideBySideBarrelDistortionStereoRendering();
            MySceneView.IsAttributionTextVisible = false;

            // USC
            var camera = new Camera(34.02209, -118.2853, 1000, 0, 45, 0);
            MySceneView.SetViewpointCamera(camera);
        }
        public void agAddDemoPoints()
        {

            var wgs84 = MySceneView.Scene.SpatialReference;
            GraphicsOverlay go = new GraphicsOverlay();
            go.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            var rnd = new System.Random();

            for (int i = 0; i < 50; i++)
            {
                var buoy1Loc = new MapPoint(-118.2606, 34.0498, 1000, wgs84);     
                var buoyMarker = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.Red, 10);          
                var buoyTest = new Graphic(buoy1Loc, buoyMarker);
                go.Graphics.Add(buoyTest);
                Task.Factory.StartNew(async () => {
                    while (true)
                    {
                        buoyTest.Geometry = new MapPoint(-118.2606 + rnd.NextDouble(), 34.0498 + rnd.NextDouble(), rnd.Next(0, 1000));
                        engine.Play3D("../../../../bus.wav", -10.0f, 0, 0, false);

                        await Task.Delay(100);
                    }
                });
            }
            MySceneView.GraphicsOverlays.Add(go);
        }

      

        public void twitter_hose()
        {

            /***
             *  DONT COMMIT ME
             */
            var creds = Auth.SetUserCredentials("a", "6zbr6iO0f", "18056YfgFhVod", "4f8XXXhCL");
            /***
             *  DONT COMMIT ME
             */

            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoice("Microsoft Zira Desktop");


            /* set up twitter stuff */
            var stream = Stream.CreateFilteredStream();
            var top_left = new Coordinates(34.035199, -118.309177);
            var bottom_right = new Coordinates(33.996693, -118.2616002);

            stream.AddLocation(top_left, bottom_right);
            stream.MatchingTweetReceived += (sender, args) =>
            {
                //XXX Todo: put this in the world somewhere, or TTS?
                //Console.WriteLine("tweet is '" + args.Tweet + "'");
                
                /* speech synth */
                synth.Speak(args.Tweet.CreatedBy.ScreenName + ": " + args.Tweet.Text);
            };

            Task.Factory.StartNew(async () =>
            {
                stream.StartStreamMatchingAllConditions();
            });
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
                    dynamic res = JsonConvert.DeserializeObject(downloadString);
                    bikes.Graphics.Clear();
                    foreach (dynamic bike in res.features)
                    {
                        var ploc = new MapPoint(bike.properties.longitude.Value, bike.properties.latitude.Value, 0, wgs84);
                        float full = (bike.properties.bikesAvailable.Value + 0.1f) / (bike.properties.totalDocks.Value + 0.1f);
                        var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.FromArgb(255, 0, (byte)(255*full), 0), 10);
                        var ptest = new Graphic(ploc, pmark);
                        bikes.Graphics.Add(ptest);
                    }

                    await Task.Delay(25000);

                }
            });

            MySceneView.GraphicsOverlays.Add(bikes);
        }

        GraphicsOverlay planes;
        public void plane_data()
        {

            WebClient client = new WebClient();
            var wgs84 = MySceneView.Scene.SpatialReference;

            planes = new GraphicsOverlay();
            planes.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;


            Task.Factory.StartNew(async () => {
                while (true)
                {
                    
                    string downloadString = client.DownloadString("http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat=34&lng=-118.28&fDstL=0&fDstU=100");
                    dynamic res = JsonConvert.DeserializeObject(downloadString);
                    planes.Graphics.Clear();
                    foreach (dynamic plane in res.acList)
                    {
                        double lo = 0;
                        double la = 0;
                        double al = 0;
                        try
                        {
                            lo = plane.Long.Value;
                            la = plane.Lat.Value;
                            if(plane.Alt != null)
                            { 
                                al = plane.Alt.Value;
                            }
                        }
                        catch (Exception)
                        {

                        }
                        var ploc = new MapPoint(lo, la, al, wgs84);
                        var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.Blue, 10);
                        var ptest = new Graphic(ploc, pmark);
                        planes.Graphics.Add(ptest);
                    }
                    
                    await Task.Delay(1250);
                    
                }
            });

            MySceneView.GraphicsOverlays.Add(planes);


        }
        ArcGISSceneLayer agsl;
        public void agAddBuilding()
        {
            Task.Factory.StartNew(async () =>
            {
                Uri u = new System.Uri("LARIAC_BUILDINGS_2014.slpk", UriKind.Relative);

                agsl = new ArcGISSceneLayer(u);
                await agsl.LoadAsync();
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        MySceneView.Scene.OperationalLayers.Add(agsl);
                    });
               
                Debug.WriteLine("building load status: " + agsl.LoadStatus);
            });
                
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
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        var stopsLayer = new Esri.ArcGISRuntime.Mapping.FeatureLayer(agsl_stop);
                        MySceneView.Scene.OperationalLayers.Add(stopsLayer);
                    });

                Debug.WriteLine("busstop: " + agsl_stop.LoadStatus);
            });
            
            /* then get vehicles */

            WebClient client = new WebClient();
            var wgs84 = MySceneView.Scene.SpatialReference;

            metro = new GraphicsOverlay();
            metro.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            Task.Factory.StartNew(async () => {
                while (true)
                {

                    string downloadString = client.DownloadString("http://api.metro.net/agencies/lametro/vehicles/");
                    dynamic res = JsonConvert.DeserializeObject(downloadString);
                    metro.Graphics.Clear();
                    foreach (dynamic vehicle in res.items)
                    {
                        var ploc = new MapPoint(vehicle.longitude.Value, vehicle.latitude.Value, 0, wgs84);
                        var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.IndianRed, 8);
                        var ptest = new Graphic(ploc, pmark);
                        metro.Graphics.Add(ptest);
                    }

                    await Task.Delay(1000);

                }
            });

            MySceneView.GraphicsOverlays.Add(metro);
        }
        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("Starting up!");

            /* set up audio stuff */
            engine = new ISoundEngine();

            /* Set up scene */
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
    }
}
