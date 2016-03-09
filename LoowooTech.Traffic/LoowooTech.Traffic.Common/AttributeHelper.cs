using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public static class AttributeHelper
    {
        /// <summary>
        /// 字段名称 对应  中文  中英对照
        /// </summary>
        private static Dictionary<string, string> FieldDict { get; set; }
        /// <summary>
        /// 字段名称  对应 在要素类中的Index
        /// </summary>
        private static Dictionary<string, int> IndexDict { get; set; }
        private static void ReadFieldIndexDict(IFeatureClass FeatureClass,string AddFieldName=null)
        {
            IndexDict = GISHelper.GetFieldIndexDict(FeatureClass,AddFieldName);
        }
        private static void Tranlate(string LayerName) 
        {
            FieldDict = LayerInfoHelper.GetLayerDictionary(LayerName);
        }
        public static IArray Identify(IFeatureClass featureClass, IGeometry geometry,string WhereClause)
        {
            if (geometry == null)
            {
                return null;
            }
            if (geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                ITopologicalOperator topop = geometry as ITopologicalOperator;
                double buffer = 0.0;
                if (double.TryParse(System.Configuration.ConfigurationManager.AppSettings["BUFFER"], out buffer))
                {
                    geometry = topop.Buffer(buffer);
                }
            }
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            IFeatureLayerDefinition featureLayerDefinition = featureLayer as IFeatureLayerDefinition;
            featureLayerDefinition.DefinitionExpression = WhereClause;
            IFeatureLayer newfeatureLayer = featureLayerDefinition.CreateSelectionLayer(featureClass.AliasName, false, null, WhereClause);
            IIdentify identify = featureLayer as IIdentify;
            IArray identifyObjs = identify.Identify(geometry);
            return identifyObjs;
        }
        public static IFeature Identify2(IFeatureClass featureClass, IGeometry geometry)
        {
            ISpatialFilter spatialfilter = new SpatialFilterClass();
            spatialfilter.Geometry = geometry;
            switch (featureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                case esriGeometryType.esriGeometryMultipoint:
                    spatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    spatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                case esriGeometryType.esriGeometryEnvelope:
                    spatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }
            spatialfilter.GeometryField = featureClass.ShapeFieldName;

            IFeatureCursor featureCursor = featureClass.Search(spatialfilter, false);
            IFeature feature = featureCursor.NextFeature();
            return feature == null ? null : feature;
        }
        public static DataTable GetTable(List<IFeature> List, Dictionary<string, int> FieldDict,out Dictionary<int,IFeature> FeatureDict)
        {
            DataTable dataTable = new DataTable();
            foreach (var key in FieldDict.Keys)
            {
                dataTable.Columns.Add(key);
            }
            DataRow dataRow = null;
            int Serial = 0;
            FeatureDict = new Dictionary<int, IFeature>();
            foreach (var item in List)
            {
                dataRow = dataTable.NewRow();
                foreach (var key in FieldDict.Keys)
                {
                    dataRow[key] = item.get_Value(FieldDict[key]).ToString();
                }
                FeatureDict.Add(Serial++, item);
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static DataTable GetTable(IFeatureClass featureClass, string Filter,out Dictionary<int,IFeature> FeatureDict,IGeometry geometry=null,SpaceMode mode=SpaceMode.Intersect)
        {
            ReadFieldIndexDict(featureClass,"序号");
            Tranlate(featureClass.AliasName.GetAlongName());
            DataTable dataTable = new DataTable();
            string temp = string.Empty;
            foreach (var key in IndexDict.Keys)
            {
                if (FieldDict.ContainsKey(key))
                {
                    temp = FieldDict[key];
                }
                else
                {
                    temp = key;
                }
                dataTable.Columns.Add(temp);
            }
            List<IFeature> FeatureList = null;
            if (geometry == null)
            {
                FeatureList = GISHelper.Search(featureClass, Filter);
            }
            else
            {
                FeatureList = GISHelper.Search(featureClass, geometry, mode);
            }
            
            DataRow dataRow = null;
            int Serial = 0;
            FeatureDict = new Dictionary<int, IFeature>();
            foreach (var feature in FeatureList)
            {
                FeatureDict.Add(Serial, feature);
                dataRow = dataTable.NewRow();
                string val = string.Empty;
                foreach (var key in IndexDict.Keys)
                {
                    val = feature.get_Value(IndexDict[key]).ToString();
                    if (FieldDict.ContainsKey(key))
                    {
                        dataRow[FieldDict[key]] = val;
                    }
                    else
                    {
                        dataRow[key] = val;
                    }
                }
                dataRow["序号"] = ++Serial;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static DataTable GetTable(IFeatureClass featureClass, IFeature feature, string LayerName)
        {
            ReadFieldIndexDict(featureClass);
            Tranlate(LayerName.GetEngish());
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("名称");
            dataTable.Columns.Add("值");
            DataRow datarow;
            foreach (var key in IndexDict.Keys)
            {
                try
                {
                    datarow = dataTable.NewRow();
                    if (FieldDict.ContainsKey(key))
                    {
                        datarow["名称"] = FieldDict[key];
                    }
                    else
                    {
                        datarow["名称"] = key;
                    }
                    
                    datarow["值"] = feature.get_Value(IndexDict[key]).ToString();
                    dataTable.Rows.Add(datarow);
                }
                catch
                {

                }   
            }
            return dataTable;
        }
        public static DataTable GetTable(List<User> List)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("序号");
            dataTable.Columns.Add("用户名");
            dataTable.Columns.Add("权限组");
            DataRow dataRow;
            foreach (var item in List)
            {
                dataRow = dataTable.NewRow();
                dataRow["序号"] = item.ID;
                dataRow["用户名"] = item.Name;
                dataRow["权限组"] = item.Role.GetDescription();
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static DataTable GetTable(List<BusLine> List)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("起始站");
            dataTable.Columns.Add("终点站");
            dataTable.Columns.Add("方向");
            DataRow dataRow;
            foreach (var item in List)
            {
                dataRow = dataTable.NewRow();
                dataRow["起始站"] = item.StartStop;
                dataRow["终点站"] = item.EndStop;
                dataRow["方向"] = item.Direction.GetDescription();
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static Dictionary<string, string> GetValues(IFeature Feature,Dictionary<string,int> FieldIndexDict)
        {
            var dict = new Dictionary<string, string>();
            foreach (var key in FieldIndexDict.Keys)
            {
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, Feature.get_Value(FieldIndexDict[key]).ToString());
                }
            }
            return dict;
        }
        
    }
}
