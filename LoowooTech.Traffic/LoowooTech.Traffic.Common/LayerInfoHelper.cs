using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

namespace LoowooTech.Traffic.Common
{
    public static class LayerInfoHelper
    {
        private static XmlDocument confiXml { get; set; }
        static LayerInfoHelper()
        {
            string LayerInfoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["LAYERINFO"]);
            //string LayerInfoPath = System.Configuration.ConfigurationManager.AppSettings["LAYERINFO"];
            confiXml = new XmlDocument();
            confiXml.Load(LayerInfoPath);
        }
        public static Dictionary<string, string> GetLayerDictionary(string LayerName)
        {
            var dict = new Dictionary<string, string>();
            var nodes = confiXml.SelectNodes("/Layers/Layer[@Name='" + LayerName + "']/Field");
            if (nodes != null)
            {
                var count = nodes.Count;
                string name = string.Empty;
                string label = string.Empty;
                for (var i = 0; i < count; i++)
                {
                    name = nodes[i].Attributes["Name"].Value;
                    label = nodes[i].Attributes["Label"].Value;
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (!dict.ContainsKey(name))
                        {
                            dict.Add(name, label);
                        }
                    }
                }
            }
            return dict;
        }
        public static Dictionary<string, string> GetValuesLabelDictionary(string LayerName, string FieldLabel)
        {
            var dict = new Dictionary<string, string>();
            var nodes = confiXml.SelectNodes("/Layers/Layer[@Name='" + LayerName + "']/Field[@Label='" + FieldLabel + "']/Value");
            if (nodes != null)
            {
                var count = nodes.Count;
                string value = string.Empty;
                string Label = string.Empty;
                for (var i = 0; i < count; i++)
                {
                    value = nodes[i].Attributes["Name"].Value;
                    Label = nodes[i].Attributes["Label"].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!dict.ContainsKey(value))
                        {
                            dict.Add(value, Label);
                        }
                    }
                }
            }
            return dict;
        }
        /// <summary>
        /// 获取相关信息下的图层列表   如路网  获取Map文件中的所有路网列表
        /// </summary>
        /// <param name="TypeName"> 路网</param>
        /// <returns></returns>
        public static List<string> GetLayers(string TypeName)
        {
            var nodes = confiXml.SelectNodes("/Layers/Way[@Type='" + TypeName + "']/Children");
            var list = new List<string>();
            if (nodes != null)
            {
                var count = nodes.Count;
                for (var i = 0; i < count; i++)
                {
                    list.Add(nodes[i].Attributes["Name"].Value);
                }
            }
            return list;
        }
        public static string GetLayer(this string TypeName)
        {
            var node = confiXml.SelectSingleNode("/Layers/Way[@Type='" + TypeName + "']/Children");
            if (node != null)
            {
                return  node.Attributes["Name"].Value;
            }
            return string.Empty;

        }
        public static List<string> GetStatistic(string StatisticName)
        {
            var nodes = confiXml.SelectNodes("/Layers/Statistics[@Name='"+StatisticName+"']/Statistic");
            var list = new List<string>();
            if (nodes != null)
            {
                var count = nodes.Count;
                for (var i = 0; i < count; i++)
                {
                    list.Add(nodes[i].Attributes["Name"].Value);
                }
            }
            return list;
        }
        public static string GetEngish(this string LayerName)
        {
            var node = confiXml.SelectSingleNode("/Layers/Way/Children[@Name='" + LayerName + "']");
            if (node != null)
            {
                node = node.ParentNode;
                return node != null ? node.Attributes["Type"].Value : null;
            }
            return null;
        }
    }
}
