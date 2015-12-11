using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    /*
    public static class SDEManager
    {
        public static IMap Map { get; set; }
        public static IFeatureClass GetFeatureClass(string layerName)
        {
            var fl = GetFeatureLayer(layerName);
             if (fl != null) return fl.FeatureClass;
             return null;
        }

        public static IFeatureLayer GetFeatureLayer(string layerName)
        {
            if (string.IsNullOrEmpty(layerName)) return null;
            for (var i = 0; i < Map.LayerCount; i++)
            {
                var lyr = Map.Layer[i];
                if (lyr is IFeatureLayer)
                {
                    if (lyr.Name == layerName) return (IFeatureLayer)lyr;
                }
                else if (lyr is ICompositeLayer)
                {
                    var gl = lyr as ICompositeLayer;
                    for (var j = 0; j < gl.Count; j++)
                    {
                        var lyr2 = gl.get_Layer(j);
                        if (lyr2.Name == layerName && lyr2 is IFeatureLayer) return (IFeatureLayer)lyr2;
                    }
                }
            }

            return null;
        }

        public static string GetAlongName(this string FullName)
        {
            return FullName.Replace("sde.SDE.", "").Trim().ToString();
        }

        public static bool AddFeature(Dictionary<string, string> FieldValDict, Dictionary<string, int> FieldIndexDict, IFeatureClass FeatureClass, IGeometry geometry)
        {
            IField tempField = null;
            int Index = 0;
            int val1 = 0;
            double val2 = 0.0;
            string temp = string.Empty;
            if (FeatureClass != null)
            {
                IFeatureBuffer featureBuffer = FeatureClass.CreateFeatureBuffer();
                featureBuffer.Shape = geometry;
                IFeatureCursor featureCursor = FeatureClass.Insert(true);
                foreach (var field in FieldValDict.Keys)
                {
                    if (FieldIndexDict.ContainsKey(field))
                    {
                        Index = FieldIndexDict[field];
                        tempField = FeatureClass.Fields.get_Field(Index);
                        temp = FieldValDict[field];
                        switch (tempField.Type)
                        {
                            case esriFieldType.esriFieldTypeString:
                                featureBuffer.set_Value(Index, temp);
                                break;
                            case esriFieldType.esriFieldTypeDouble:
                                if (double.TryParse(temp, out val2))
                                {
                                    featureBuffer.set_Value(Index, val2);
                                }

                                break;
                            case esriFieldType.esriFieldTypeInteger:
                                if (int.TryParse(temp, out val1))
                                {
                                    featureBuffer.set_Value(Index, val1);
                                }

                                break;
                            default:
                                try
                                {
                                    featureBuffer.set_Value(Index, temp);
                                }
                                catch
                                {

                                }
                                break;
                        }
                    }
                }
                try
                {
                    object featureOID = featureCursor.InsertFeature(featureBuffer);
                    featureCursor.Flush();
                }
                catch
                {
                    return false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
                return true;
            }
            return false;
        }

        public static bool EditFeature(Dictionary<string, string> FieldValDict, Dictionary<string, int> FieldIndexDict, IFeature Feature)
        {
            IField tempField = null;
            int Index = 0;
            int val1 = 0;
            double val2 = 0.0;
            string temp = string.Empty;
            foreach (var field in FieldValDict.Keys)
            {
                if (FieldIndexDict.ContainsKey(field))
                {
                    Index = FieldIndexDict[field];
                    tempField = Feature.Fields.get_Field(Index);
                    temp = FieldValDict[field];
                    switch (tempField.Type)
                    {
                        case esriFieldType.esriFieldTypeString:
                            Feature.set_Value(Index, temp);
                            break;
                        case esriFieldType.esriFieldTypeDouble:
                            if (double.TryParse(temp, out val2))
                            {
                                Feature.set_Value(Index, val2);
                            }

                            break;
                        case esriFieldType.esriFieldTypeInteger:
                            if (int.TryParse(temp, out val1))
                            {
                                Feature.set_Value(Index, val1);
                            }

                            break;
                        default:
                            try
                            {
                                Feature.set_Value(Index, temp);
                            }
                            catch
                            {

                            }
                            break;
                    }

                }
            }
            Feature.Store();
            return false;
        }
    }*/

    
    public static class SDEManager
    {
        private static string Server { get; set; }
        private static string Instance { get; set; }
        private static string User { get; set; }
        private static string Password { get; set; }
        private static string Database { get; set; }
        private static string Version { get; set; }
        private static IWorkspace SDEWorkspace { get; set; }

        static SDEManager()
        {
            Server = System.Configuration.ConfigurationManager.AppSettings["SERVER"];
            Instance = System.Configuration.ConfigurationManager.AppSettings["INSTANCE"];
            User = System.Configuration.ConfigurationManager.AppSettings["USER"];
            Password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"];
            Database = System.Configuration.ConfigurationManager.AppSettings["DATABASE"];
            Version = System.Configuration.ConfigurationManager.AppSettings["VERSION"];
        }

        public static bool Connect()
        {
            SDEWorkspace = OpenSde();
            return SDEWorkspace != null;
        }
        
        private static IWorkspace arcSDEWorkspaceOpen(string server, string instance, string user, string password, string database, string version)
        {
            IPropertySet pPropertySet = new PropertySetClass();
            pPropertySet.SetProperty("SERVER", server);
            pPropertySet.SetProperty("INSTANCE", instance);
            pPropertySet.SetProperty("USER", user);
            pPropertySet.SetProperty("PASSWORD", password);
            pPropertySet.SetProperty("DATABASE", database);
            pPropertySet.SetProperty("VERSION", version);
            IWorkspaceFactory2 pWorkspaceFactory = new SdeWorkspaceFactoryClass();
            IWorkspace workspace = null;
            try
            {
                workspace = pWorkspaceFactory.Open(pPropertySet, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接Sde时发生错误：" + ex.Message);
                return null;
            }
            return workspace;
        }
        
        private static IWorkspace OpenSde()
        {
            return arcSDEWorkspaceOpen(Server, Instance, User, Password, Database, Version);
        }

        public static IFeatureClass GetFeatureClass(string FeatureClassName)
        {
            if (SDEWorkspace == null)
            {
                Console.WriteLine("SDEWorkspace为null，无法进行获取要素类.......");
                return null;
            }
            IFeatureWorkspace featureWorkspace = SDEWorkspace as IFeatureWorkspace;
            IFeatureClass featureClass = null;
            try
            {
                featureClass = featureWorkspace.OpenFeatureClass(FeatureClassName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取要素类时，发生错误："+ex.Message);
            }
            return featureClass;
        }

        public static string GetAlongName(this string FullName)
        {
            return FullName.Replace("sde.SDE.", "").Trim().ToString();
        }

        public static bool AddFeature(Dictionary<string, string> FieldValDict, Dictionary<string, int> FieldIndexDict, IFeatureClass FeatureClass, IGeometry geometry)
        {
            IField tempField = null;
            int Index = 0;
            int val1 = 0;
            double val2 = 0.0;
            string temp = string.Empty;
            if (FeatureClass != null)
            {
                IFeatureBuffer featureBuffer = FeatureClass.CreateFeatureBuffer();
                featureBuffer.Shape = geometry;
                IFeatureCursor featureCursor = FeatureClass.Insert(true);
                foreach (var field in FieldValDict.Keys)
                {
                    if (FieldIndexDict.ContainsKey(field))
                    {
                        Index = FieldIndexDict[field];
                        tempField = FeatureClass.Fields.get_Field(Index);
                        temp = FieldValDict[field];
                        switch (tempField.Type)
                        {
                            case esriFieldType.esriFieldTypeString:
                                featureBuffer.set_Value(Index, temp);
                                break;
                            case esriFieldType.esriFieldTypeDouble:
                                if (double.TryParse(temp, out val2))
                                {
                                    featureBuffer.set_Value(Index, val2);
                                }

                                break;
                            case esriFieldType.esriFieldTypeInteger:
                                if (int.TryParse(temp, out val1))
                                {
                                    featureBuffer.set_Value(Index, val1);
                                }

                                break;
                            default:
                                try
                                {
                                    featureBuffer.set_Value(Index, temp);
                                }
                                catch
                                {

                                }
                                break;
                        }
                    }
                }
                try
                {
                    object featureOID = featureCursor.InsertFeature(featureBuffer);
                    featureCursor.Flush();
                }
                catch
                {
                    return false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
                return true;
            }
            return false;
        }

        public static bool EditFeature(Dictionary<string, string> FieldValDict, Dictionary<string, int> FieldIndexDict, IFeature Feature)
        {
            IField tempField = null;
            int Index = 0;
            int val1 = 0;
            double val2 = 0.0;
            string temp = string.Empty;
            foreach (var field in FieldValDict.Keys)
            {
                if (FieldIndexDict.ContainsKey(field))
                {
                    Index = FieldIndexDict[field];
                    tempField = Feature.Fields.get_Field(Index);
                    temp = FieldValDict[field];
                    switch (tempField.Type)
                    {
                        case esriFieldType.esriFieldTypeString:
                            Feature.set_Value(Index, temp);
                            break;
                        case esriFieldType.esriFieldTypeDouble:
                            if (double.TryParse(temp, out val2))
                            {
                                Feature.set_Value(Index, val2);
                            }

                            break;
                        case esriFieldType.esriFieldTypeInteger:
                            if (int.TryParse(temp, out val1))
                            {
                                Feature.set_Value(Index, val1);
                            }

                            break;
                        default:
                            try
                            {
                                Feature.set_Value(Index, temp);
                            }
                            catch
                            {

                            }
                            break;
                    }

                }
            }
            Feature.Store();
            return false;
        }

    }
}
