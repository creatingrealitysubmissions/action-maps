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

namespace arcGIS_test_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // la is 34.02209° N, -118.2852° E
        Graphic buoyTest;

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("Starting up!");
            
            MySceneView.Scene = new Scene(Basemap.CreateTopographic());
             
            // Add San Diego scene layer.  Example scene layers provided by Esri available here: http://www.arcgis.com/home/group.html?id=c4a19ab700fd4654b89a319b016eee03
            //MySceneView.Scene.OperationalLayers.Add(new ArcGISSceneLayer(new System.Uri("https://tiles.arcgis.com/tiles/Imiq6naek6ZWdour/arcgis/rest/services/San_Diego_Textured_Buildings/SceneServer/layers/0")));
            MySceneView.Scene.OperationalLayers.Add(new ArcGISSceneLayer(new System.Uri("http://scene.arcgis.com/arcgis/rest/services/Hosted/Buildings_Brest/SceneServer/layers/0")));

            // Add elevation surface from ArcGIS Online
            MySceneView.Scene.BaseSurface.ElevationSources.Add(new ArcGISTiledElevationSource(new System.Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer")));

            // Define rendering mode for VR experience.  
            //MySceneView.StereoRendering = new SideBySideBarrelDistortionStereoRendering();
            MySceneView.IsAttributionTextVisible = false;

            
            var camera = new Camera(34.02209, -118.2853, 1000, 0, 45, 0);
            MySceneView.SetViewpointCamera(camera);

            var wgs84 = MySceneView.Scene.SpatialReference;

            // USC is 34.02209° N, -118.2852° E
            // intercontinental is 34.0498° N, -118.2606° E
            var buoy1Loc = new MapPoint(-118.2606, 34.0498, 1000, wgs84);
            

            // create a marker symbol
            var buoyMarker = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.Red, 10);

            // create graphics
            var buoyTest = new Graphic(buoy1Loc, buoyMarker);

            GraphicsOverlay go = new GraphicsOverlay();
            go.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            go.Graphics.Add(buoyTest);
            MySceneView.GraphicsOverlays.Add(go);

            /* Make the intercontinental marked */
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    buoyTest.Geometry = new MapPoint(-118.2606, 34.0498, (new System.Random()).Next(0, 1000));
                    await Task.Delay(100);
                }
            });

            /* Load Bus stops */
            Task.Factory.StartNew(async () =>
            {


                // bus stops
                Uri u = new System.Uri("https://services1.arcgis.com/gOY38HDnFUYPDony/arcgis/rest/services/la_busstops1217/FeatureServer");
                ServiceFeatureTable sft = new ServiceFeatureTable(u);


                await sft.LoadAsync();

                MySceneView.Scene.OperationalLayers.Add(new FeatureLayer(sft));

            });
        }

        
    }
}
