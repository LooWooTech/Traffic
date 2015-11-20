using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
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
        private static void ReadDict(IFeatureClass FeatureClass)
        {
            FieldDict = LayerInfoHelper.GetLayerDictionary(FeatureClass.AliasName.GetAlongName());
            if (IndexDict == null)
            {
                IndexDict = new Dictionary<string, int>();
            }
            int Index = 0;
            foreach (var key in FieldDict.Keys)
            {
                Index = FeatureClass.Fields.FindField(key);
                if (Index >= 0)
                {
                    if (!IndexDict.ContainsKey(key))
                    {
                        IndexDict.Add(key, Index);
                    }
                }
            }
        }
        private static void ReadFieldIndexDict(IFeatureClass FeatureClass)
        {
            IndexDict = GISHelper.GetFieldIndexDict(FeatureClass);
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
                geometry = topop.Buffer(0.0001);
            }
            IFeatureLayer featureLayer = new FeatureLayerClass();
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
        public static DataTable GetTable(IFeatureClass featureClass, string Filter)
        {
            ReadFieldIndexDict(featureClass);
            DataTable dataTable = new DataTable(); 
            dataTable.Columns.Add("序号");
            foreach (var key in IndexDict.Keys)
            {
                dataTable.Columns.Add(key);
            }
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = Filter;
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            DataRow dataRow = null;
            int Serial = 1;
            while (feature != null)
            {
                try
                {
                    dataRow = dataTable.NewRow();
                    dataRow["序号"] = Serial++;
                    foreach (var key in IndexDict.Keys)
                    {
                        dataRow[key] = feature.get_Value(IndexDict[key]).ToString();
                    }
                    dataTable.Rows.Add(dataRow);
                }
                catch
                {

                }
                feature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return dataTable;
        }
        public static DataTable GetTable(IFeatureClass featureClass, IFeature feature, string LayerName)
        {
            ReadDict(featureClass);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("名称");
            dataTable.Columns.Add("值");
            DataRow datarow;
            foreach (var key in FieldDict.Keys)
            {
                try
                {
                    datarow = dataTable.NewRow();
                    datarow["名称"] = FieldDict[key];
                    datarow["值"] = feature.get_Value(IndexDict[key]).ToString();
                    dataTable.Rows.Add(datarow);
                }
                catch
                {

                }   
            }
            return dataTable;
        }
    }
}
