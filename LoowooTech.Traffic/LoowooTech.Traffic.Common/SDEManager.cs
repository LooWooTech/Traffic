using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Common
{
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
            SDEWorkspace = OpenSde();
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

        /// <summary>
        /// 获取要素类
        /// </summary>
        /// <param name="FeatureClassName">要素类名称</param>
        /// <returns>要素类</returns>
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
        public static Dictionary<string, int> GetFieldIndex(IFeatureClass featureClass, Dictionary<string, string> FDict)
        {
            var dict = new Dictionary<string, int>();
            int Index = 0;
            foreach (var key in FDict.Keys)
            {
                try
                {
                    Index = featureClass.Fields.FindField(key);
                }
                catch
                {
                    continue;
                }
                
                if (Index > 0)
                {
                    if (!dict.ContainsKey(key))
                    {
                        dict.Add(key, Index);
                    }
                }
            }
            return dict;
        }
        public static string GetAlongName(this string FullName)
        {
            return FullName.Replace("sde.SDE.", "").Trim().ToString();
        }

    }
}
