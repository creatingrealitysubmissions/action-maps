﻿using System;
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
using Esri.ArcGISRuntime.UI;

namespace arcGIS_test_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // la is 34.02209° N, -118.2852° E
        public MainWindow()
        {
            InitializeComponent();

            MySceneView.Scene = new Scene(Basemap.CreateTopographic());
            // Add San Diego scene layer.  Example scene layers provided by Esri available here: http://www.arcgis.com/home/group.html?id=c4a19ab700fd4654b89a319b016eee03
            //MySceneView.Scene.OperationalLayers.Add(new ArcGISSceneLayer(new System.Uri("https://tiles.arcgis.com/tiles/Imiq6naek6ZWdour/arcgis/rest/services/San_Diego_Textured_Buildings/SceneServer/layers/0")));
            MySceneView.Scene.OperationalLayers.Add(new ArcGISSceneLayer(new System.Uri("http://scene.arcgis.com/arcgis/rest/services/Hosted/Buildings_Brest/SceneServer/layers/0")));

            // Add elevation surface from ArcGIS Online
            MySceneView.Scene.BaseSurface.ElevationSources.Add(new ArcGISTiledElevationSource(new System.Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer")));

            // Define rendering mode for VR experience.  
            MySceneView.StereoRendering = new SideBySideBarrelDistortionStereoRendering();
            MySceneView.IsAttributionTextVisible = false;

            var camera = new Camera(34.02209, -118.2853, 1000, 0, 45, 0);
            MySceneView.SetViewpointCamera(camera);

        }
    }
}
