using ESRI.ArcGIS.Geodatabase;
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
        public static  Dictionary<string,int> GetFieldIndexDict(IFeatureClass FeatureClass)
        {
            var dict = new Dictionary<string, int>();
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
    }
}
