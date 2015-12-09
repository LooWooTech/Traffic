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
        private INewPolygonFeedback newPolygonFeedback { get; set; }
        private AxMapControl axMapControl { get; set; }
        private MainForm Father { get; set; }
        public FrameSearchTool(AxMapControl axMapControl,MainForm Father)
        {
            this.Father = Father;
            this.axMapControl = axMapControl;
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
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            var pg = axMapControl.TrackPolygon();
            return;
        }
        public override void OnDblClick()
        {
            
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
