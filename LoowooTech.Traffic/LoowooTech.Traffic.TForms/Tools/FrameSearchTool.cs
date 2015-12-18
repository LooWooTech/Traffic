using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using LoowooTech.Traffic.Common;
using ESRI.ArcGIS.Display;

namespace LoowooTech.Traffic.TForms.Tools
{
    [Guid("1D3FB4A1-B4CD-45F6-AB1F-208CEFC68336")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Info.ExpTool")]
    public sealed class FrameSearchTool:BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            var regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            var regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper mHookHelper { get; set; }
        private AxMapControl axMapControl { get; set; }
        private MainForm Father { get; set; }
        private string LayerName { get; set; }
        private bool StatisticFlag { get; set; }
        public IFeatureLayer FeatureLayer
        {
            get
            {
                for (var i = 0; i < mHookHelper.FocusMap.LayerCount; i++)
                {
                    var tLayer = mHookHelper.FocusMap.get_Layer(i);
                    if (tLayer is GroupLayer)
                    {
                        var layer = tLayer as ICompositeLayer;
                        if (layer == null) continue;
                        for (var j = 0; j < layer.Count; j++)
                        {
                            var featureLayer = layer.get_Layer(j) as IFeatureLayer;
                            if (featureLayer.Name == LayerName)
                            {
                                return featureLayer;
                            }
                        }
                    }
                    else if (tLayer is FeatureLayer)
                    {
                        if ((tLayer as IFeatureLayer).Name == LayerName)
                        {
                            return tLayer as IFeatureLayer;
                        }
                    }
                }
                return null;
            }
        }
        public FrameSearchTool(AxMapControl axMapControl,MainForm Father,string LayerName,bool Statistic=false)
        {
            this.Father = Father;
            this.axMapControl = axMapControl;
            this.LayerName = LayerName;
            this.StatisticFlag = Statistic;
        }

        public override void OnCreate(object hook)
        {
            try
            {
                mHookHelper = new HookHelperClass { Hook = hook };
                if (mHookHelper.ActiveView == null)
                {
                    mHookHelper = null;
                }
            }
            catch
            {
                mHookHelper = null;
            }
        }

        private void Filter(IGeometry geometry)
        {
            if (FeatureLayer != null&&geometry!=null&&geometry is Polygon)
            {
                Father.Invoke(new EventOperator(Father.Analyze), new[] { geometry });
            }
        }
        private void StatisticAnalyze(IGeometry geometry)
        {
            if (geometry != null && geometry is Polygon)
            {
                Father.SelectFeature(geometry, Models.SpaceMode.Contains, FeatureLayer); 
                Dictionary<int,IFeature> temp;
                var data = AttributeHelper.GetTable(FeatureLayer.FeatureClass, null, out temp, geometry, Models.SpaceMode.Contains);
                var ParkingKey = System.Configuration.ConfigurationManager.AppSettings["PARKINGKEY1"];
                var dict = ExcelHelper.Statistic(data, ParkingKey);
                var sum = ExcelHelper.Statistic2(data, System.Configuration.ConfigurationManager.AppSettings["PARKINGKEY2"]);
                var form = new StatisticsForm(dict, "当前框选区域停车设施情况", sum,Father.ParkingName,ParkingKey);
                form.ShowDialog();
                //var features = GISHelper.Search(FeatureLayer.FeatureClass, geometry, Models.SpaceMode.Contains);
            }
        }
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            axMapControl.Map.ClearSelection();
            
            var pg = axMapControl.TrackPolygon();
            if (StatisticFlag)
            {
                Father.Invoke(new EventOperator(StatisticAnalyze), new[] { pg });
            }
            else
            {
                Filter(pg);
            }
            
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            
        }

        public override bool Deactivate()
        {
            return true;
        }
            

    }
}
