using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime;
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
            MySceneView.Scene = new Scene(Basemap.CreateTopographic());
            MySceneView.Scene.OperationalLayers.Add(new ArcGISSceneLayer(new System.Uri("https://tiles.arcgis.com/tiles/Imiq6naek6ZWdour/arcgis/rest/services/San_Diego_Textured_Buildings/SceneServer/layers/0")));
            MySceneView.Scene.OperationalLayers.Add(new ArcGISSceneLayer(new System.Uri("http://services.arcgis.com/V6ZHFr6zdgNZuVG0/ArcGIS/rest/services/LABuildings_3D/SceneServer")));
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

            /* set up twitter stuff */
            var stream = Stream.CreateFilteredStream();
            var top_left = new Coordinates(34.035199, -118.309177);
            var bottom_right = new Coordinates(33.996693, -118.2616002);


            stream.AddLocation(top_left, bottom_right);
            stream.MatchingTweetReceived += (sender, args) =>
            {
                //XXX Todo: put this in the world somewhere, or TTS?
                Console.WriteLine("tweet is '" + args.Tweet + "'");
            };

            stream.StartStreamMatchingAllConditions();
        }
        GraphicsOverlay planes;
        public void plane_data()
        {

            WebClient client = new WebClient();
            string downloadString = client.DownloadString("http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat=34&lng=-118.28&fDstL=0&fDstU=100");
            dynamic res = JsonConvert.DeserializeObject(downloadString);

            var wgs84 = MySceneView.Scene.SpatialReference;
            planes = new GraphicsOverlay();
            planes.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;

            foreach (dynamic plane in res.acList)
            {
                var ploc = new MapPoint(plane.Long.Value, plane.Lat.Value, plane.Alt.Value, wgs84);
                var pmark = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.Blue, 10);
                var ptest = new Graphic(ploc, pmark);
                planes.Graphics.Add(ptest);
            }
            MySceneView.GraphicsOverlays.Add(planes);
        }

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("Starting up!");

            /* set up audio stuff */
            engine = new ISoundEngine();

            /* Set up scene */
            agSetupScene();

            /* Demo Dots */
            //agAddDemoPoints();

            /* Start twitter stream */
            //twitter_hose();

            /* Grab the plane data */
            plane_data();
            /* Grab the metro data */

            /* Grab the bikeshare data */
           
        }
    }
}
