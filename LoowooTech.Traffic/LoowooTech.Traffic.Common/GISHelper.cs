using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using LoowooTech.Traffic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public static class GISHelper
    {
        /// <summary>
        /// 获取要素类中的所有字段名称
        /// </summary>
        /// <param name="FeatureClass">要素类</param>
        /// <returns></returns>
        public static  Dictionary<string,int> GetFieldIndexDict(IFeatureClass FeatureClass,string AddFieldName=null)
        {
            var dict = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(AddFieldName))
            {
                dict.Add(AddFieldName, 0);
            }
            var count = FeatureClass.Fields.FieldCount;
            IField field = null;
            for (var i = 0; i < count; i++)
            {
                field = FeatureClass.Fields.get_Field(i);
                dict.Add(field.Name,i);
            }
            return dict;
        }
        /// <summary>
        /// 获取要素类中字段名称对应的字段类型字典
        /// </summary>
        /// <param name="FeatureClass">要素类</param>
        /// <returns></returns>
        public static Dictionary<string, esriFieldType> GetFieldDict(IFeatureClass FeatureClass)
        {
            var dict = new Dictionary<string, esriFieldType>();
            var count = FeatureClass.Fields.FieldCount;
            IField field = null;
            string Name = string.Empty;
            for (var i = 0; i < count; i++)
            {
                field = FeatureClass.Fields.get_Field(i);
                Name = field.Name;
                if (!string.IsNullOrEmpty(Name))
                {
                    if (!dict.ContainsKey(Name))
                    {
                        dict.Add(Name, field.Type);
                    }
                }
            }
            return dict;
        }
        private static IFeatureClass Create(IFeatureWorkspace FeatureWorkspace, string Name, ESRI.ArcGIS.Geometry.esriGeometryType esriGeometryType,Dictionary<string,esriFieldType> FieldDict)
        {
            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
            IField pField = new FieldClass();
            IFieldEdit pFieldEdit = pField as IFieldEdit;
            pFieldEdit.Name_2 = "shape";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            IGeometryDef pGeometryDef = new GeometryDefClass();
            IGeometryDefEdit pGeometryDefEdit = pGeometryDef as IGeometryDefEdit;
            pGeometryDefEdit.GeometryType_2 = esriGeometryType;

            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            pSpatialReference.SetDomain(-6000000, 6000000, -6000000, 6000000);
            pGeometryDefEdit.SpatialReference_2 = pSpatialReference;
            pFieldEdit.GeometryDef_2 = pGeometryDef;
            pFieldsEdit.AddField(pField);

            pField = new FieldClass();
            pFieldEdit = pField as IFieldEdit;
            pFieldEdit.Name_2 = "FID";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            pFieldsEdit.AddField(pField);

            foreach (var key in FieldDict.Keys)
            {
                if (key.ToUpper() == "SHAPE"||key.ToUpper()=="OBJECTID")
                {
                    continue;
                }
                pField = new FieldClass();
                pFieldEdit = pField as IFieldEdit;
                pFieldEdit.Name_2 = key;
                pFieldEdit.Type_2 = FieldDict[key];
                pFieldsEdit.AddField(pField);
            }


            IFeatureClassDescription featureClassDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription objectClassDescription = featureClassDescription as IObjectClassDescription;
            IFeatureClass featureClass = FeatureWorkspace.CreateFeatureClass(Name, pFields, objectClassDescription.InstanceCLSID, objectClassDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");
            return featureClass;

        }
        public static void Save2(IFeatureClass SourceFeatureClass,string WhererClause,string SaveFilePath)
        {
            Geoprocessor gp = new Geoprocessor();
            FeatureClassToFeatureClass tool = new FeatureClassToFeatureClass();
            tool.in_features = SourceFeatureClass;
            tool.where_clause = WhererClause;
            tool.out_path = System.IO.Path.GetDirectoryName(SaveFilePath);
            tool.out_name = System.IO.Path.GetFileNameWithoutExtension(SaveFilePath);
            gp.Execute(tool, null);
        }
        public static void WBuffer(IFeature Feature)
        {
            Geoprocessor gp = new Geoprocessor();
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(Feature, @"F:\Github\Traffic\LoowooTech.Traffic\LoowooTech.Traffic\bin\Debug\Temp\output.shp", "1 Kilometers");
            gp.Execute(buffer, null);
        }

        
        public static IGeometry WMerge(this IGeometry Sgeometry,IGeometry geometry)
        {
            IGeometryCollection geometryCollection = new GeometryBagClass();
            geometryCollection.AddGeometry(Sgeometry);
            geometryCollection.AddGeometry(geometry);
            ITopologicalOperator unionedpolygon = new PolygonClass();
            unionedpolygon.ConstructUnion(geometryCollection as IEnumGeometry);
            return unionedpolygon as IGeometry;
            //IArray array = new ArrayClass();
            //array.Add(Sgeometry);
            //array.Add(geometry);
            //IBasicGeoprocessor basicGeoprocessor = new BasicGeoprocessorClass();
            ////basicGeoprocessor.Merge(array,)
            //IGeometry result = new PolygonClass();
            //Geoprocessor gp = new Geoprocessor();
            //ESRI.ArcGIS.DataManagementTools.Merge merge = new ESRI.ArcGIS.DataManagementTools.Merge();
            //merge.inputs = Sgeometry + ";" + geometry;
            //merge.output = result;
            //gp.Execute(merge, null);
            //return result;
        }
        public static void Save(IFeatureClass SourceFeatureClass, string WhereClause, string SaveFilePath)
        {
            IWorkspaceFactory workpsaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = workpsaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(SaveFilePath), 0);
            var FieldDict = GetFieldDict(SourceFeatureClass);
            IFeatureWorkspace FeatureWorkspace = workspace as IFeatureWorkspace;
            IFeatureClass NewFeatureClass = Create(FeatureWorkspace, System.IO.Path.GetFileNameWithoutExtension(SaveFilePath), esriGeometryType.esriGeometryPolyline, FieldDict);
            Dictionary<string, int> NewIndexDict = GetFieldIndexDict(NewFeatureClass);
            Dictionary<string, int> SourceIndexDict = GetFieldIndexDict(SourceFeatureClass);
            IQueryFilter queryFilter = null;
            if (!string.IsNullOrEmpty(WhereClause))
            {
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = WhereClause;
            }
            IFeatureBuffer featureBuffer;
            IFeatureCursor InsertFeatureCursor=null;
            IFeatureCursor SearchfeatureCursor = SourceFeatureClass.Search(queryFilter, false);
            IFeature feature = SearchfeatureCursor.NextFeature();
            while (feature != null)
            {
                featureBuffer = NewFeatureClass.CreateFeatureBuffer();
                InsertFeatureCursor = NewFeatureClass.Insert(true);
                featureBuffer.Shape = feature.Shape;
                foreach (var key in NewIndexDict.Keys)
                {
                    if (SourceIndexDict.ContainsKey(key))
                    {
                        object val = feature.get_Value(SourceIndexDict[key]);
                        if (val!=DBNull.Value)
                        {
                            featureBuffer.set_Value(NewIndexDict[key], val);
                        }
                        
                    }
                }
                InsertFeatureCursor.InsertFeature(featureBuffer);
                InsertFeatureCursor.Flush();
                feature = SearchfeatureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(SearchfeatureCursor);
            if (InsertFeatureCursor != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(InsertFeatureCursor);
            }
            
        }
        /// <summary>
        /// 一般搜索
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <param name="WhereClause"></param>
        /// <returns></returns>
        public static List<IFeature> Search(IFeatureClass FeatureClass, string WhereClause)
        {
            var list = new List<IFeature>();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = WhereClause;
            IFeatureCursor featureCursor = FeatureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                list.Add(feature);
                feature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return list;
        }

        public static void Statistic(IFeatureClass stopfeatureClass,string stopwhereClause,string fieldName,out double maxValue,out double minValue)
        {
            var queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = stopwhereClause;
            var featureCursor = stopfeatureClass.Search(queryFilter, false);
            var statisticCursor = featureCursor as ICursor;
            var dataStatistic = new DataStatisticsClass();
            dataStatistic.Cursor = statisticCursor;
            dataStatistic.Field = fieldName;
            var statisticsResult = dataStatistic.Statistics;
            maxValue = statisticsResult.Maximum;
            minValue = statisticsResult.Minimum;
        }
        public static IFeature Search2(IFeatureClass FeatureClass, string WhereClause)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = WhereClause;
            IFeatureCursor featureCursor = FeatureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return feature;
        }
        
        /// <summary>
        /// 空间搜索
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <param name="geometry"></param>
        /// <param name="spaceMode"></param>
        /// <returns></returns>
        public static List<IFeature> Search(IFeatureClass FeatureClass, IGeometry geometry, SpaceMode spaceMode)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.GeometryField = "shape";
            spatialFilter.Geometry = geometry;
            switch (spaceMode)
            {
                case SpaceMode.Touches:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                    break;
                case SpaceMode.Overlaps:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                    break;
                case SpaceMode.Intersect:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
                case SpaceMode.Crossed:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case SpaceMode.Contains:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case SpaceMode.Within:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    break;
            }
            queryFilter = spatialFilter as IQueryFilter;
            IFeatureCursor featureCursor = FeatureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            var list = new List<IFeature>();
            while (feature != null)
            {
                list.Add(feature);
                feature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return list;
        }

        /// <summary>
        /// 获取要素类中某一个字段中的唯一属性值
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <returns></returns>
        public static List<string> GetUniqueValue(IFeatureClass FeatureClass,string FieldName)
        {
            var list = new List<string>();
            IDataset dataset = FeatureClass as IDataset;
            IFeatureWorkspace featureWorkspace = dataset.Workspace as IFeatureWorkspace;

            IQueryDef queryDef = featureWorkspace.CreateQueryDef();
            queryDef.Tables = dataset.Name;
            queryDef.SubFields = "DISTINCT (" + FieldName + ")";
            ICursor cursor = queryDef.Evaluate();
            int Index = FeatureClass.Fields.FindField(FieldName);
            if (Index != -1)
            {
                IField field = FeatureClass.Fields.get_Field(Index);
                IRow row = cursor.NextRow();
                while (row != null)
                {
                    if (!string.IsNullOrEmpty(row.get_Value(0).ToString()))
                    {
                        list.Add(row.get_Value(0).ToString());
                    }
                    
                    row = cursor.NextRow();
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursor);
            return list;
        }

        public static Dictionary<string,double> Statistic(IFeatureClass FeatureClass,string LabelName ,string FieldName,IGeometry Geometry=null)
        {
            var dict = new Dictionary<string, double>();
            var Index = FeatureClass.FindField(LabelName);
            if (Index != -1)
            {
                IField field = FeatureClass.Fields.get_Field(Index);
                var valList = GetUniqueValue(FeatureClass, LabelName);//获取标注字段的唯一值
                foreach (var val in valList)
                {
                    if (!dict.ContainsKey(val) && !string.IsNullOrEmpty(val))
                    {
                        if (field.Type == esriFieldType.esriFieldTypeString)
                        {
                            dict.Add(val, Statistic2(FeatureClass, FieldName, LabelName + " = '" + val+"'",null,Geometry));
                        }
                        else
                        {
                            dict.Add(val, Statistic2(FeatureClass, FieldName, LabelName + " = " + val,null,Geometry));
                        }
                    }
                }
            }
            return dict;
        }
        
        public static Dictionary<string, double> Statistic(IFeatureClass FeatureClass, StatisticMode Mode)
        {
            
            if (Mode == StatisticMode.Region)
            {
                return Statistic(FeatureClass, "SIX");
            }
            else if (Mode == StatisticMode.Restrict)
            {
                return Statistic(FeatureClass, "CF");
            }
            var dict = new Dictionary<string, double>();
            var list = LayerInfoHelper.GetStatistic(Mode.GetDescription());
            foreach (var item in list)
            {
                if (!dict.ContainsKey(item) && !string.IsNullOrEmpty(item))
                {
                    dict.Add(item, Statistic2(FeatureClass, "LENGTH", "DISTRICT='" + item + "' AND RANK <> '匝道' AND RANK <> '连杆道路' AND RANK <> '步行街'", "DISTRICT='" + item + "' AND RANK='快速路'"));
                }
            }
            return dict;
        }
        public static Dictionary<string, double> Statistic(IFeatureClass FeatureClass,string FeatureClassName)
        {
            var dict = new Dictionary<string, double>();
            IFeatureClass RegionFeatureClass = SDEManager.GetFeatureClass(System.Configuration.ConfigurationManager.AppSettings[FeatureClassName]);
            int Serial = RegionFeatureClass.Fields.FindField("name");
            if (RegionFeatureClass != null && Serial != -1)
            {
                IFeatureCursor featureCursor = RegionFeatureClass.Search(null, false);
                IFeature feature = featureCursor.NextFeature();
                string Name = string.Empty;
                while (feature != null)
                {
                    Name = feature.get_Value(Serial).ToString().Trim();
                    if (!string.IsNullOrEmpty(Name) && !dict.ContainsKey(Name))
                    {
                        dict.Add(Name, Statistic2(FeatureClass, "LENGTH", feature, "RANK <> '匝道' AND RANK <> '连杆道路' AND RANK <> '步行街'") - Statistic2(FeatureClass, "LENGTH", feature, "RANK='快速路'"));
                    }
                    feature = featureCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            }
            return dict;
        }
        public static double Statistic2(IFeatureClass FeatureClass, string FieldName,IFeature Feature,string WhererClause)
        {

            var spatialFilter = new SpatialFilterClass
            {
                Geometry = Feature.Shape,
                WhereClause = WhererClause,
            };

            var rels = new []{esriSpatialRelEnum.esriSpatialRelIntersects, esriSpatialRelEnum.esriSpatialRelContains };
            IFeatureCursor featureCursor = null;
            ICursor cursor = null;
            double statisticVal = 0.0;
            foreach(var rel in rels)
            {
                spatialFilter.SpatialRel = rel;
                featureCursor = FeatureClass.Search(spatialFilter, false);
                cursor = featureCursor as ICursor;
                
               
                IDataStatistics dataStatistic;
                IStatisticsResults statisticResults;
                if (cursor != null)
                {
                    dataStatistic = new DataStatisticsClass();
                    dataStatistic.Cursor = cursor;
                    dataStatistic.Field = FieldName;
                    statisticResults = dataStatistic.Statistics;
                    statisticVal += statisticResults.Sum;
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return statisticVal;
        }

        public static double Statistic2(IFeatureClass FeatureClass, string FieldName,string WhereClause,string WhereClause2=null,IGeometry Geometry=null)
        {
            IFeatureCursor featureCursor = null;
            IQueryFilter queryFilter = new QueryFilterClass();
            if (Geometry == null)
            {
                queryFilter.WhereClause = WhereClause;
                featureCursor = FeatureClass.Search(queryFilter, false);
            }
            else
            {
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.WhereClause = WhereClause;
                spatialFilter.Geometry = Geometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                featureCursor = FeatureClass.Search(spatialFilter, false);
            }
            
            ICursor cursor = featureCursor as ICursor;
            double statisticVal = 0.0;
            IDataStatistics dataStatistic;
            IStatisticsResults statisticsResluts;
            if (cursor != null)
            {
                dataStatistic = new DataStatisticsClass();
                dataStatistic.Cursor = cursor;
                dataStatistic.Field = FieldName;
                statisticsResluts = dataStatistic.Statistics;
                statisticVal=statisticsResluts.Sum;
            }
            if (!string.IsNullOrEmpty(WhereClause2))
            {
                queryFilter.WhereClause = WhereClause2;
                featureCursor = FeatureClass.Search(queryFilter, false);
                cursor = featureCursor as ICursor;
                if (cursor != null)
                {
                    dataStatistic = new DataStatisticsClass();
                    dataStatistic.Cursor = cursor;
                    dataStatistic.Field = FieldName;
                    statisticsResluts = dataStatistic.Statistics;
                    statisticVal -= statisticsResluts.Sum / 2;
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return statisticVal;
        }
        public static string GetBusStopWhereClause(List<IFeature> List, int Index1,int Index2)
        {
            string WhereClause = string.Empty;
            string LineId_bus = string.Empty;
            string lineDirect = string.Empty;
            foreach (var feature in List)
            {
                LineId_bus = feature.get_Value(Index1).ToString().Trim();
                lineDirect = feature.get_Value(Index2).ToString().Trim();
                if (!string.IsNullOrEmpty(LineId_bus)&&!string.IsNullOrEmpty(lineDirect))
                {
                    if (string.IsNullOrEmpty(WhereClause))
                    {
                        WhereClause += "lineIdBus = " + LineId_bus + " AND lineDirect = " + lineDirect + " ";
                    }
                    else
                    {
                        WhereClause += "OR lineIdBus = " + LineId_bus + " AND lineDirect = " + lineDirect + " "; 
                    }
                }
            }
            return WhereClause;
        }
        public static Dictionary<string, IFeature> GetWhereClauseFeature(IFeatureClass FeatureClass,string FieldName,string WhereClause=null)
        {
            var dict = new Dictionary<string, IFeature>();
            var list = GetUniqueValue(FeatureClass, FieldName);
            string NewWhereClause = string.Empty;
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(WhereClause))
                {
                    NewWhereClause = FieldName + " = " + item;
                }
                else
                {
                    NewWhereClause = WhereClause + " AND " + FieldName + " = " + item;
                }
                IFeature feature = Search2(FeatureClass, NewWhereClause);
                if (feature != null)
                {
                    if (!dict.ContainsKey(NewWhereClause))
                    {
                        dict.Add(NewWhereClause, feature);
                    }
                }
            }
            return dict;
        }

        public static string GetValue(IFeature Feature, int Index)
        {
            string values = string.Empty;
            try
            {
                values = Feature.get_Value(Index).ToString();
            }
            catch
            {

            }
            return values;
        }
        public static List<FeatureResult> GetRoadList(IFeatureClass RoadFeatureClass, IFeatureClass StopFeatureClass, string Key)
        {
            var list = new List<FeatureResult>();
            string RoadKey1=System.Configuration.ConfigurationManager.AppSettings["BUSKEY"];//lineName
            string RoadKey2=System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY2"];//lineDirect
            string StopKey=System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY1"];//lineNameshort
            var StopKey2 = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY4"];//lineIdBus
            var valList = GetUniqueValue(RoadFeatureClass, RoadKey2);
            var KeyIndex = RoadFeatureClass.Fields.FindField(System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY3"]);//LineId_bus
            var StopKey3 = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY5"];//stopIndex
            string RoadWhereClause = string.Empty;
            string StopWhereClause = string.Empty;
            double MaxValue = .0;
            double MinValue = .0;
            string LineId = "";
            foreach (var item in valList)
            {
                RoadWhereClause = string.Format("{0} = '{1}' AND {2} = {3} ", RoadKey1, Key,RoadKey2, item);
                IFeature feature = Search2(RoadFeatureClass, RoadWhereClause);
                if (feature != null)
                {
                    LineId = GetValue(feature, KeyIndex);
                    StopWhereClause = string.Format("{0} = {1} AND {2} = {3}", StopKey2, LineId,RoadKey2,item);
                    Statistic(StopFeatureClass, StopWhereClause, StopKey3, out MaxValue, out MinValue);
                    list.Add(new FeatureResult()
                    {
                        RoadWhereClause = RoadWhereClause,
                        Feature = feature,
                        StopWhereClause = StopWhereClause,
                        Stops = Search(StopFeatureClass, StopWhereClause),
                        StartEndWhereClause =string.Format("({0} = {1} AND {2} = {3} ) AND ({4} = {5} OR {4} = {6})",StopKey2,LineId,RoadKey2,item,StopKey3,MaxValue,MinValue)
                    });
                }
            }
            return list;
        }
        public static string GetStartEndWhereClause(IFeatureClass roadFeatureClass,string roadWhereClause,IFeatureClass stopFeatureClass)
        {
            var whereClause = string.Empty;
            var features = Search(roadFeatureClass, roadWhereClause);
            var roadBusIdIndex = roadFeatureClass.Fields.FindField(System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY3"]);
            var roadBusId_bus = string.Empty;
            var stopBusIdField = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY4"];
            var LineDirect = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY2"];
            var LineDirectIndex = roadFeatureClass.Fields.FindField(LineDirect);
            var LineDirectValue = string.Empty;
            var stopIndexField = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY5"];
            double MaxValue = .0;
            double MinValue = .0;
            var stopWhereClause = string.Empty;
            foreach(var feature in features)
            {
                roadBusId_bus = GetValue(feature, roadBusIdIndex);
                LineDirectValue = GetValue(feature, LineDirectIndex);
                stopWhereClause = string.Format("{0} = {1} AND {2} = {3}", stopBusIdField, roadBusId_bus, LineDirect, LineDirectValue);
                Statistic(stopFeatureClass, stopWhereClause, stopIndexField, out MaxValue, out MinValue);
                if (string.IsNullOrEmpty(whereClause))
                {
                    whereClause += string.Format("{0} = {1} OR {0} = {2}", stopIndexField, MaxValue, MinValue);
                }
                else
                {
                    whereClause += string.Format(" OR {0} = {1} OR {0} = {2}", stopIndexField, MaxValue, MinValue);
                }
            }
            return whereClause;
        }
        
        
        

    }
}
