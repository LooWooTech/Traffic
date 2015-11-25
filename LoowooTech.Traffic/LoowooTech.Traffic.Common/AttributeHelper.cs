
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Models;
using ESRI.ArcGIS.Carto;
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
        /// 字段名称 对应  中文
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
        
        public static IArray Identify(IFeatureClass featureClass, IGeometry geometry)
        {
            if (geometry == null)
            {
                return null;
            }
            if (geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                ITopologicalOperator topop = geometry as ITopologicalOperator;
                geometry = topop.Buffer(0.0004);
            }
            IFeatureLayer featureLayer =new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
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
        public static DataTable GetTable(IFeatureClass featureClass, string Filter,out Dictionary<int,IFeature> FeatureDict)
        {
            ReadFieldIndexDict(featureClass,"序号");
            DataTable dataTable = new DataTable(); 
            foreach (var key in IndexDict.Keys)
            {
                dataTable.Columns.Add(key);
            }
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = Filter;
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            DataRow dataRow = null;
            int Serial = 0;
            FeatureDict = new Dictionary<int, IFeature>();
            while (feature != null)
            {
                FeatureDict.Add(Serial, feature);
                dataRow = dataTable.NewRow();

                foreach (var key in IndexDict.Keys)
                {
                    dataRow[key] = feature.get_Value(IndexDict[key]).ToString();
                }
                dataRow["序号"] = ++Serial;
                dataTable.Rows.Add(dataRow);
                
                feature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return dataTable;
        }
        public static DataTable GetTable(IFeatureClass featureClass, IFeature feature, string LayerName)
        {
            ReadFieldIndexDict(featureClass);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("名称");
            dataTable.Columns.Add("值");
            DataRow datarow;
            foreach (var key in IndexDict.Keys)
            {
                try
                {
                    datarow = dataTable.NewRow();
                    datarow["名称"] = key;
                    datarow["值"] = feature.get_Value(IndexDict[key]).ToString();
                    dataTable.Rows.Add(datarow);
                }
                catch
                {

                }   
            }
            return dataTable;
        }
        /*
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
        }*/
        
    }
}
