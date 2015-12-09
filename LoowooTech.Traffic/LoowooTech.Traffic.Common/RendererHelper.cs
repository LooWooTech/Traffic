using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public static class RendererHelper
    {
        public static List<string> GetRenderersFile()
        {
            string Folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            var list = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(Folder);
            foreach (FileInfo children in dir.GetFiles("*.lyr"))
            {
                list.Add(children.FullName);
            }
            return list;
        }
        public static MapControl GetMapControl()
        {
            MapControl mapControl = new MapControlClass();
            var list = GetRenderersFile();
            foreach (var item in list)
            {
                mapControl.AddLayerFromFile(item);
            }
            return mapControl;
        }
        public static IFeatureLayer GetFeatureLayer(MapControl MapControl,string LayerName)
        {
            IFeatureLayer featureLayer=null;
            for (var i = 0; i < MapControl.Map.LayerCount; i++)
            {
                featureLayer = MapControl.get_Layer(i) as IFeatureLayer;
                if (featureLayer.Name == LayerName)
                {
                    return featureLayer;
                }
            }
                
            return featureLayer;
        }

        public static IFeatureRenderer GetRenderer(MapControl mapControl, string LayerName)
        {
            IFeatureLayer featureLayer = GetFeatureLayer(mapControl, LayerName);
            IGeoFeatureLayer geoFeatureLayer = featureLayer as IGeoFeatureLayer;
            return geoFeatureLayer.Renderer;

        }
    }
}
