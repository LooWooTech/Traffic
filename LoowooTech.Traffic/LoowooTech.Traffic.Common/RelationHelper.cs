using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LoowooTech.Traffic.Common
{
    public static class RelationHelper
    {
        private static XmlDocument confiXml { get; set; }
        static RelationHelper()
        {
            string relationInfoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["RELATION"]);
            confiXml = new XmlDocument();
            confiXml.Load(relationInfoPath);
        }
        public static  List<string> GetRelations(string TypeName)
        {
            var nodes = confiXml.SelectNodes("/Types/Type[@Name='" + TypeName + "']/Relation");
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
        public static string GetSQLChar(this string Name)
        {
            switch (Name)
            {
                case "等于":
                    return "=";
                case "不等于":
                    return "<>";
                case "包含":
                    return "Like";
                case "大于":
                    return ">";
                case "小于":
                    return "<";
                case "大于等于":
                    return ">=";
                case "小于等于":
                    return "<=";
                case "并且":
                    return "AND";
                case "或者":
                    return "OR";
                default: return string.Empty;
            }
        }
    }
}
