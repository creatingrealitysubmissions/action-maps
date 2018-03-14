using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace arcGIS_test_3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();

            MySceneView.Scene = new Scene(Basemap.CreateDarkGrayCanvasVector());
            MySceneView.Scene.BaseSurface.ElevationSources.Add(new ArcGISTiledElevationSource(new System.Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer")));

            // Define rendering mode for VR experience.
            MySceneView.StereoRendering = new SideBySideBarrelDistortionStereoRendering();
            MySceneView.IsAttributionTextVisible = false;

            var camera = new Camera(34.02209, -118.2853, 1000, 0, 45, 0);
            MySceneView.SetViewpointCamera(camera);

            var wgs84 = MySceneView.Scene.SpatialReference;
            GraphicsOverlay go = new GraphicsOverlay();
            go.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            var rnd = new System.Random();

            for (int i = 0; i < 50; i++)
            {


                var buoy1Loc = new MapPoint(-118.2606, 34.0498, 1000, wgs84);

                // create a marker symbol
                var buoyMarker = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.Red, 10);

                // create graphics
                var buoyTest = new Graphic(buoy1Loc, buoyMarker);

                go.Graphics.Add(buoyTest);

                /* Make the intercontinental marked */
                Task.Factory.StartNew(async () => {
                    while (true)
                    {
                        buoyTest.Geometry = new MapPoint(-118.2606 + rnd.NextDouble(), 34.0498 + rnd.NextDouble(), rnd.Next(0, 1000));
                        await Task.Delay(100);
                    }
                });

            }
            MySceneView.GraphicsOverlays.Add(go);
        }
    }
}
