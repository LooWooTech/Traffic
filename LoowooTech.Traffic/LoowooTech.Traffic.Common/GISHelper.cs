using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
