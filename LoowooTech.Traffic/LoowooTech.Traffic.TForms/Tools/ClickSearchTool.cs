using System;
using System.Runtime.InteropServices;
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
    [Guid("7DD372FA-4B16-4783-870C-5DE1B7229F73")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Info.ExpTool")]
    public sealed class ClickSearchTool : BaseTool
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

        private IHookHelper mHookHelper;
        private string LayerName { get; set; }
        private string WhererClause { get; set; }
        private AxMapControl axMapControl { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        private MainForm Father { get; set; }
        private bool BusFlag { get; set; }
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
        public ClickSearchTool(string LayerName, string WhereClause, AxMapControl axMapControl,MainForm Father,bool Bus=false)
        {
            this.LayerName = LayerName;
            this.WhererClause = WhereClause;
            this.axMapControl = axMapControl;
            this.Father = Father;
            this.BusFlag = Bus;
            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 99);
            simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            simpleMarkerSymbol.Size = 8;
            simpleMarkerSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 0);
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
        private void Twinkle(IFeature feature)
        {
            if (feature == null) return;
            Twinkle(feature.Shape);
        } 
        private void Twinkle(IGeometry geo)
        {
            if (geo == null) return;

            switch (geo.GeometryType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    axMapControl.FlashShape(geo, 4, 300, simpleMarkerSymbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    axMapControl.FlashShape(geo, 4, 300, simpleLineSymbol);
                    break;
            }
        }
        private void ShowAttribute(IGeometry geometry)
        {
            if (FeatureLayer != null)
            {
                IArray array = AttributeHelper.Identify(FeatureLayer.FeatureClass, geometry, WhererClause);
                if (array != null)
                {
                    IFeatureIdentifyObj featureIdentifyObj = array.get_Element(0) as IFeatureIdentifyObj;
                    IIdentifyObj identifyObj = featureIdentifyObj as IIdentifyObj;
                    IRowIdentifyObject rowIdentifyObj = featureIdentifyObj as IRowIdentifyObject;
                    IFeature feature = rowIdentifyObj.Row as IFeature;
                    if (feature != null)
                    {
                        Twinkle(feature);
                        if (this.BusFlag)
                        {
                            axMapControl.Map.ClearSelection();
                            Father.Analyze2(feature);
                        }
                        else
                        {
                            AttributeForm form = new AttributeForm(feature, FeatureLayer.FeatureClass, LayerName);
                            form.ShowDialog(this.Father);
                        }
                       
                    }
                }
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            var pt = mHookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            ShowAttribute(pt as IGeometry);
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
