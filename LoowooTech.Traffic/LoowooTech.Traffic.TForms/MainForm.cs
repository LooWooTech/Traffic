using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.Manager;
using LoowooTech.Traffic.Models;
using LoowooTech.Traffic.TForms.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.Traffic.TForms
{
    public delegate void EventOperator(IGeometry geometry);
    public delegate void EventOperator2(string FilePath);
    public delegate void EventOperator3(IFeature Feature);
    public partial class MainForm : Form
    {
        #region  工具
        private void UncheckAllButtons(object sender)
        {
            var ctrls = new[] { btnPointer, btnZoomIn, btnZoomOut, btnIdentify, btnPan };
            foreach (var ctrl in ctrls)
            {
                ctrl.Checked = ctrl == sender;
            }

        }
        private void btnPan_Click(object sender, EventArgs e)
        {
            UncheckAllButtons(sender);
            var cmd = new ControlsMapPanTool();
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            UncheckAllButtons(sender);
            
            var cmd = new ControlsMapZoomInToolClass();
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            UncheckAllButtons(sender);
           
            var cmd = new ControlsMapZoomOutToolClass();
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            axMapControl1.Refresh();
        }
        private void btnGlobe_Click(object sender, EventArgs e)
        {
            axMapControl1.Extent = axMapControl1.FullExtent;
        }
        private void btnIdentify_Click(object sender, EventArgs e)
        {
            UncheckAllButtons(sender);

            var cmd = new ControlsMapIdentifyToolClass();
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        private void btnPointer_Click(object sender, EventArgs e)
        {
            UncheckAllButtons(sender);

            var cmd = new ControlsSelectToolClass();
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        #endregion

        #region  初始化
        private string MXDPath { get; set; }
        private string RoadName { get; set; }
        private string RoadHistoryName { get; set; }
        private string RoadNodeName { get; set; }
        public string BusLineName { get; set; }
        public string BusStopName { get; set; }
        public string ParkingName { get; set; }
        private string BikeName { get; set; }
        private string FlowName { get; set; }
        public string StartEndName { get; set; }
        private string XZQName { get; set; }
        public  string RoadFilterWhereClause { get; set; }
        public string BusLineWhereClause { get; set; }
        public string BusStopWhereClause { get; set; }
        public string ParkingWhereClause { get; set; }
        public string BikeWhereClause { get; set; }
        public string FlowWhereClause { get; set; }
        private string MapType { get; set; }
        private double Expand { get; set; }
        private IFeatureClass RoadFeatureClass { get; set; }
        private IFeatureClass RoadHistoryFeatureClass { get; set; }
        private IFeatureClass RoadNodeFeatureClass { get; set; }
        public IFeatureClass BusLineFeatureClass { get; set; }
        public IFeatureClass BusStopFeatureClass { get; set; }
        private IFeatureClass ParkingFeatureClass { get; set; }
        private IFeatureClass BikeFeatureClass { get; set; }
        private IFeatureClass FlowFeatureClass { get; set; }
        public IFeatureClass StartEndFeatureClass { get; set; }
        private IFeatureClass XZQFeatureClass { get; set; }
        private IFeature ExtentFeature { get; set; }
        private INewPolygonFeedback newPolygonFeedback { get; set; }
        private MapControl MapControl { get; set; }
        public  InquiryMode inquiryMode { get; set; }
        public DataType dataType { get; set; }
        public OperateMode operateMode { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        public User CurrentUser { get; set; }
        public SplashForm Splash { get; set; }
        public IGeometry TempGeometry { get; set; }
        public int StartField { get; set; }
        public int EndField { get; set; }

        private readonly List<IPoint> m_Crossroads = new List<IPoint>();
        private IMarkerSymbol m_CrossroadSymbol;
        private List<IPolyline> m_ImportRoads;
        private ILineSymbol m_ImportRoadSymbol;

        public MainForm()
        {
            InitializeComponent();
            MapControl = RendererHelper.GetMapControl();
            MXDPath = ConfigurationManager.AppSettings["MXD"];
            RoadName = ConfigurationManager.AppSettings["ROAD"];
            RoadHistoryName = ConfigurationManager.AppSettings["ROADHISTORY"];
            RoadNodeName = ConfigurationManager.AppSettings["ROADNODE"];
            BusLineName = ConfigurationManager.AppSettings["BUSLINE"];
            BusStopName = ConfigurationManager.AppSettings["BUSSTOP"];
            ParkingName = ConfigurationManager.AppSettings["PARKING"];
            BikeName = ConfigurationManager.AppSettings["BIKE"];
            FlowName = ConfigurationManager.AppSettings["FLOW"];
            StartEndName = ConfigurationManager.AppSettings["BusSE"];
            XZQName = ConfigurationManager.AppSettings["XZQ"];
            MapType = ConfigurationManager.AppSettings["MAPTYPE"];
            
            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 99);
            simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            simpleMarkerSymbol.Size = 8;
            simpleMarkerSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 0);

            m_CrossroadSymbol = new SimpleMarkerSymbolClass()
            {
                Style = esriSimpleMarkerStyle.esriSMSCircle,
                Size = 8,
                Color = DisplayHelper.GetRGBColor(255, 0, 0, 0),
                OutlineColor = DisplayHelper.GetRGBColor(255, 0, 0),
                OutlineSize = 3,
                Outline = true
            };

            m_ImportRoadSymbol = new SimpleLineSymbolClass()
            {
                Style = esriSimpleLineStyle.esriSLSSolid,
                Width = 3,
                Color = DisplayHelper.GetRGBColor(0, 0, 0, 200)
            };
            axMapControl1.OnAfterDraw += this.axMapControl1_OnAfterDraw;
        }

        public void Power()
        {
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.Role == Role.Common)
                {
                    this.ribbonPanel6.Visible = false;
                    this.ribbonPanel10.Visible = false;
                    this.ribbonPanel14.Visible = false;
                    this.ribbonTab6.Visible = false;
                }
            }
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            if(SDEManager.Connect() == false)
            {
                if (Splash != null) Splash.TopMost = false;
                MessageBox.Show("连接数据库错误，请联系系统维护人员。");
                if (Splash != null) Splash.TopMost = true;
            
            }
            try
            {
                string mxdPath1 = System.IO.Path.Combine(Application.StartupPath, MXDPath);
                axMapControl1.LoadMxFile(System.IO.Path.Combine(Application.StartupPath, MXDPath));
            }
            catch (Exception ex)
            {
                if (Splash != null) Splash.TopMost = false;
                MessageBox.Show("载入地图错误:" + ex.ToString());
                if (Splash != null) Splash.TopMost = true;
            }
            

            axTOCControl1.SetBuddyControl(axMapControl1);

            RoadFeatureClass = SDEManager.GetFeatureClass(RoadName);
            RoadHistoryFeatureClass = SDEManager.GetFeatureClass(RoadHistoryName);
            RoadNodeFeatureClass = SDEManager.GetFeatureClass(RoadNodeName);
            BusLineFeatureClass = SDEManager.GetFeatureClass(BusLineName);
            BusStopFeatureClass = SDEManager.GetFeatureClass(BusStopName);
            ParkingFeatureClass = SDEManager.GetFeatureClass(ParkingName);
            BikeFeatureClass = SDEManager.GetFeatureClass(BikeName);
            FlowFeatureClass = SDEManager.GetFeatureClass(FlowName);
            StartEndFeatureClass = SDEManager.GetFeatureClass(StartEndName);
            XZQFeatureClass = SDEManager.GetFeatureClass(XZQName);
            StartField = BusLineFeatureClass.FindField(System.Configuration.ConfigurationManager.AppSettings["START"]);
            EndField = BusLineFeatureClass.FindField(System.Configuration.ConfigurationManager.AppSettings["END"]);
            ExtentFeature = GISHelper.Search2(SDEManager.GetFeatureClass(System.Configuration.ConfigurationManager.AppSettings["SCALE"]), null);
            Expand = double.Parse(System.Configuration.ConfigurationManager.AppSettings["Expand"]);
            if (Splash != null)
            {
                Splash.panel1.Visible = true;
                Splash.progressBar1.Visible = false;
                this.Enabled = false;
            }
            
        }
        #endregion

        #region  MapControl  操作


        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            var viewDrawPhase = (esriViewDrawPhase)e.viewDrawPhase;
            //if (viewDrawPhase == esriViewDrawPhase.esriViewForeground)
            {

                object o = m_ImportRoadSymbol;
                if (m_ImportRoads != null)
                {
                    foreach(var line in m_ImportRoads)
                    axMapControl1.DrawShape(line, ref o);
                }

                object r = m_CrossroadSymbol;
                foreach (var pt in m_Crossroads)
                {
                    axMapControl1.DrawShape(pt, ref r);
                }


            }
        }
        #endregion

        #region  地图显示更新

        private void ribbon1_ActiveTabChanged(object sender, EventArgs e)
        {
            this.MapType = "Base";
            axMapControl1.Map.ClearSelection();
            DataType currentData = ribbon1.ActiveTab.Text.GetDataEnum();
            SaveInit(sender);
            DataRefesh(currentData);
        }
      
        private void DataRefesh(DataType dataType)
        {
            var list = new List<string>();
            switch (dataType)
            {
                case DataType.Road:
                    list.Add(RoadName.GetLayer());
                    break;
                case DataType.BusLine:
                    list.Add(BusLineName.GetLayer());
                    list.Add(BusStopName.GetLayer());
                    break;
                case DataType.Flow:
                    list.Add(FlowName.GetLayer());
                    break;
                case DataType.Parking:
                    list.Add(ParkingName.GetLayer());
                    break;
                case DataType.Bike:
                    list.Add(BikeName.GetLayer());
                    break;
                case DataType.People:
                    break;
            }
            if (dataType != DataType.Road)
            {
                list.Add(System.Configuration.ConfigurationManager.AppSettings["RoadBackground"]);
            }
            ILayer layer = null;
            IFeatureLayer featureLayer = null;
            string Ignore = System.Configuration.ConfigurationManager.AppSettings["Ignore"];
            short opacity = short.Parse(System.Configuration.ConfigurationManager.AppSettings["OPACITY2"]);
            for (var i = 0; i < axMapControl1.Map.LayerCount; i++)
            {
                layer = axMapControl1.Map.get_Layer(i);
                if (layer.Name == Ignore)
                {
                    continue;
                }
                if (layer is GroupLayer)
                {
                    ICompositeLayer compositeLayer = layer as ICompositeLayer;
                    
                    for (var j = 0; j < compositeLayer.Count; j++)
                    {
                        featureLayer = compositeLayer.get_Layer(j) as IFeatureLayer;
                        if (list.Contains(featureLayer.Name))
                        {
                            featureLayer.Visible = true;
                            ILayerEffects layerEffects = featureLayer as ILayerEffects;
                            layerEffects.Transparency = featureLayer.Name == BusStopName.GetLayer() ? opacity : featureLayer.Name == BusLineName.GetLayer() ? opacity : (short)0;
                        }
                        else
                        {
                            featureLayer.Visible = false;
                        }
                    }
                }
                else if (layer is IFeatureLayer)
                {
                    featureLayer = layer as IFeatureLayer;
                    if (list.Contains(featureLayer.Name))
                    {
                        featureLayer.Visible = true;
                        ILayerEffects layerEffects = featureLayer as ILayerEffects;
                        layerEffects.Transparency = featureLayer.Name == BusLineName.GetLayer() ? opacity : featureLayer.Name == BusStopName.GetLayer() ? opacity : (short)0;
                    }
                    else
                    {
                        featureLayer.Visible = false;
                    }
                }
            }
            if (ExtentFeature != null)
            {
                axMapControl1.Extent = ExtentFeature.Shape.Envelope;
            }
            axMapControl1.ActiveView.Refresh();
        }
        public void ConditionControlCenter()
        {
            string WhereClause = string.Empty;
            string LayerName = string.Empty;
            IFeatureClass CurrentFeatureClass = null;
            bool Flag=false;
            switch (this.dataType)
            {
                case DataType.Road:
                    RoadFilterWhereClause = toolStripStatusLabel1.Text;
                    WhereClause = RoadFilterWhereClause;
                    LayerName = RoadName;
                    CurrentFeatureClass = RoadFeatureClass;
                    break;
                case DataType.Parking:
                    ParkingWhereClause = toolStripStatusLabel1.Text;
                    WhereClause = ParkingWhereClause;
                    LayerName = ParkingName;
                    CurrentFeatureClass = ParkingFeatureClass;
                    break;
                case DataType.Bike:
                    BikeWhereClause = toolStripStatusLabel1.Text;
                    WhereClause = BikeWhereClause;
                    LayerName = BikeName;
                    CurrentFeatureClass = BikeFeatureClass;
                    break;
                case DataType.Flow:
                    FlowWhereClause = toolStripStatusLabel1.Text;
                    WhereClause = FlowWhereClause;
                    LayerName = FlowName;
                    CurrentFeatureClass = FlowFeatureClass;
                    break;
                case DataType.BusLine:
                    BusLineWhereClause = toolStripStatusLabel1.Text;
                    WhereClause = BusLineWhereClause;
                    LayerName = BusLineName;
                    CurrentFeatureClass = BusLineFeatureClass;
                    Flag = true;
                    break;
                case DataType.BusStop:
                    BusStopWhereClause = toolStripStatusLabel1.Text;
                    WhereClause = BusStopWhereClause;
                    LayerName = BusStopName;
                    CurrentFeatureClass = BusStopFeatureClass;
                    break;
            }
            if (this.dataType == DataType.BusLine&&this.inquiryMode==InquiryMode.Filter)
            {
                var list = GISHelper.Search(CurrentFeatureClass, WhereClause);
                BusStopWhereClause = GISHelper.GetBusStopWhereClause(list,  CurrentFeatureClass.Fields.FindField("ShortName"), CurrentFeatureClass.Fields.FindField("lineDirect"));
                UpdateBase(BusStopName, BusStopWhereClause, BusStopFeatureClass);
            }
            switch (this.inquiryMode)
            {
                case InquiryMode.Filter://
                    UpdateBase(LayerName, WhereClause,CurrentFeatureClass,Flag);
                    break;
                case InquiryMode.Search:
                    ShowResult(CurrentFeatureClass, WhereClause);
                    break;
                case InquiryMode.Region:

                    break;
                case InquiryMode.Way:
                    break;
            }
        }
        public void UpdateBase(string Name,string WhereClause,IFeatureClass FeatureClass,bool Flag=false,bool BFlag=false)
        {
            var CurrentLayerName = Name.GetLayer();
            IFeatureLayer featureLayer = GetFeatureLayer(CurrentLayerName); 
            short opacity = Flag?(short)0 :short.Parse(System.Configuration.ConfigurationManager.AppSettings["OPACITY2"]);
            if (featureLayer != null)
            {
                IFeatureLayerDefinition featureLayerDefinition = featureLayer as IFeatureLayerDefinition;
                featureLayerDefinition.DefinitionExpression = WhereClause;
                Center(FeatureClass, WhereClause);
                ILayerEffects layerEffects = featureLayer as ILayerEffects;
                if (BFlag)
                {
                    layerEffects.Transparency = (short)0;
                }
                else
                {
                    layerEffects.Transparency = featureLayer.Name == BusStopName.GetLayer() ? opacity : featureLayer.Name == BusLineName.GetLayer() ? opacity : (short)0;
                }
                if (featureLayer.Name == BusStopName.GetLayer() && Flag)
                {
                    IGeoFeatureLayer geoFeatureLayer = featureLayer as IGeoFeatureLayer;
                    IAnnotateLayerPropertiesCollection annotateLayerPropertiesCollection = geoFeatureLayer.AnnotationProperties;
                    for (var i = 0; i < annotateLayerPropertiesCollection.Count; i++)
                    {
                        
                    }
                }
                axMapControl1.Map.ClearSelection();
                axMapControl1.ActiveView.Refresh();
            }  
        }
        private string GetValue(IFeature Feature, int Index)
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
        public string GetStartEndWhereClause(FeatureResult featurResult)
        {
            return "stopName= '" + GetValue(featurResult.Feature, StartField) + "' Or stopName ='" + GetValue(featurResult.Feature, EndField) + "'";
        }
        public void OpenClose(string Name, bool Flag)
        {
            ILayer layer = null;
            for (var i = 0; i < axMapControl1.Map.LayerCount; i++)
            {
                layer = axMapControl1.Map.get_Layer(i);
                if (layer is GroupLayer)
                {
                    ICompositeLayer compositeLayer = layer as ICompositeLayer;
                    IFeatureLayer tempFeatureLayer = null;
                    for (var j = 0; j < compositeLayer.Count; j++)
                    {
                        tempFeatureLayer = compositeLayer.get_Layer(j) as IFeatureLayer;
                        if (tempFeatureLayer.Name.Trim().ToUpper() == Name)
                        {
                            tempFeatureLayer.Visible = Flag;
                        }
                    }
                }
            }
            axMapControl1.ActiveView.Refresh();
        }
        public void UpdateStartEnd(List<FeatureResult> List,string Name,IFeatureClass FeatureClass)
        {
            string WhereClause = GetStartEndWhereClause(List[0]);
            IFeatureLayer featurelayer = GetFeatureLayer(Name);
            if (featurelayer != null)
            {
                OpenClose(StartEndName, true);
                IFeatureLayerDefinition featurelayerDefinition = featurelayer as IFeatureLayerDefinition;
                featurelayerDefinition.DefinitionExpression = WhereClause;
                axMapControl1.Map.ClearSelection();
                axMapControl1.ActiveView.Refresh();
            }
        }
        #endregion
        public void ShowResult(IFeatureClass FeatureClass,string WhereClause)
        {
            AttributeForm2 form2 = new AttributeForm2(FeatureClass, WhereClause);
            form2.Show(this);
        }
        public void MapRefresh()
        {
            this.axMapControl1.ActiveView.Refresh();
            this.axTOCControl1.Update();
        }

        #region  要素操作  居中闪烁
        /// <summary>
        /// 要素闪烁
        /// </summary>
        /// <param name="Feature"></param>
        public void Twinkle(IFeature Feature)
        {
            if (Feature == null) return;
            Twinkle(Feature.Shape);                
        }

        public void Twinkle(IGeometry geo)
        {
            if (geo == null) return;

            switch (geo.GeometryType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    axMapControl1.FlashShape(geo, 4, 300, simpleMarkerSymbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    axMapControl1.FlashShape(geo, 4, 300, simpleLineSymbol);
                    break;
            }
        }
        #region IFeatureSelection
        //public void Twinkle(IFeatureLayer FeatureLayer, string WhereClause)
        //{
        //    IQueryFilter queryfilter = new QueryFilterClass();
        //    queryfilter.WhereClause = WhereClause;
        //    IFeatureSelection featureSelection = FeatureLayer as IFeatureSelection;
        //    featureSelection.SelectFeatures(queryfilter, esriSelectionResultEnum.esriSelectionResultAnd, false);

        //    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, axMapControl1.ActiveView.Extent);
           
        //    if (featureSelection.SelectionSet.Count > 0)
        //    {
        //        IEnumFeature enumFeature = axMapControl1.Map.FeatureSelection as IEnumFeature;
        //        IFeature Feature = enumFeature.Next();
        //        IEnvelope envelope = new Envelope() as IEnvelope;
        //        while (Feature != null)
        //        {
        //            envelope.Union(Feature.Extent);
        //            Feature = enumFeature.Next();
        //        }
        //        axMapControl1.ActiveView.Extent = envelope;
        //        axMapControl1.ActiveView.Refresh();
        //    }
        //}
        #endregion

        public void CenterBase(IEnvelope envelope)
        {
            IPoint point = new PointClass();
            try
            {
                point.PutCoords((envelope.XMin + envelope.XMax) / 2, (envelope.YMin + envelope.YMax) / 2);
            }
            catch
            {
                envelope = axMapControl1.ActiveView.Extent;
                point.PutCoords((envelope.XMin + envelope.XMax) / 2, (envelope.YMin + envelope.YMax) / 2);
            }
            
            //axMapControl1.CenterAt(point);
            //居中方法二

            envelope.Expand(this.Expand, this.Expand, true);

            var env2 = axMapControl1.ActiveView.Extent;
            env2.CenterAt(point);
            axMapControl1.ActiveView.Extent = envelope;//env2  时  当前视图显示范围

            axMapControl1.ActiveView.Refresh();
        }
        /// <summary>
        /// 居中  一个要素
        /// </summary>
        /// <param name="Feature"></param>
        public void Center(IFeature Feature)
        {
            if (Feature != null)
            {
                if (Feature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                {
                    var env2 = this.ExtentFeature.Extent;
                    env2.CenterAt(Feature.Shape as IPoint);
                    axMapControl1.ActiveView.Extent = env2;
                    axMapControl1.ActiveView.Refresh();
                }
                else
                {
                    CenterBase(Feature.Shape.Envelope);
                }
                
            }
        }

        public void Center(IGeometry geo)
        {
            if(geo != null)
            {
                if (geo.GeometryType == esriGeometryType.esriGeometryPoint) 
                {
                    var env2 = this.ExtentFeature.Extent;
                    env2.CenterAt(geo as IPoint);
                    axMapControl1.ActiveView.Extent = env2;
                    axMapControl1.ActiveView.Refresh();
                }
                else
                {
                    CenterBase(geo.Envelope);
                }

                
            }
        }
        /// <summary>
        /// 居中
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <param name="WhereClause"></param>
        public void Center(IFeatureClass FeatureClass, string WhereClause)
        {
            IEnvelope envelope = new Envelope() as IEnvelope;
            if (!string.IsNullOrEmpty(WhereClause))
            {
                IQueryFilter queryfilter = new QueryFilterClass();
                queryfilter.WhereClause = WhereClause;
                IFeatureCursor featurecursor = FeatureClass.Search(queryfilter, false);
                IFeature feature = featurecursor.NextFeature();
                while (feature != null)
                {
                    envelope.Union(feature.Extent);
                    feature = featurecursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featurecursor);
            }
            else
            {
                envelope = this.ExtentFeature.Shape.Envelope;
                //envelope = axMapControl1.FullExtent;
            }
            
            CenterBase(envelope);
            
        }
        #endregion

        #region 导出Shapefile文件
        private void ExportSHPBase(IFeatureClass FeatureClass, string WhereClause, string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                OperatorTxt.Text = "开始导出Shapefile文件";
                try
                {
                    GISHelper.Save2(FeatureClass, WhereClause, FilePath);
                }
                catch (Exception ex)
                {
                    OperatorTxt.Text = "导出Shapefile文件失败，错误信息：" + ex.ToString();
                    return;
                }
                OperatorTxt.Text = "成功导出shap文件：" + FilePath;
            }
        }

        private void ExportRoadSHP_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出路网SHP文件", "shp文件|*.shp");
            ExportSHPBase(RoadFeatureClass, RoadFilterWhereClause, saveFilePathSHP);
        }

        private void ExportBusLineSHP_Click(object sender, EventArgs e)
        {
            var saveShpPath = FileHelper.Save("导出公交车路线Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BusLineFeatureClass, BusLineWhereClause, saveShpPath);
        }

        private void ExportBusStopSHP_Click(object sender, EventArgs e)
        {
            var saveSHPPath = FileHelper.Save("导出公交车站点Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BusStopFeatureClass, BusStopWhereClause, saveSHPPath);
        }

        private void ExportParkingSHP_Click(object sender, EventArgs e)
        {
            var saveSHPPath = FileHelper.Save("导出停车设施Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(ParkingFeatureClass, ParkingWhereClause, saveSHPPath);
        }
        private void ExportFlowSHP_Click(object sender, EventArgs e)
        {
            var saveSHPPath = FileHelper.Save("导出交通流量监测器Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(FlowFeatureClass, FlowWhereClause, saveSHPPath);
        }
        private void ExportBikeSHP_Click(object sender, EventArgs e)
        {
            var saveSHPPath = FileHelper.Save("导出公共自行车Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BikeFeatureClass, BikeWhereClause, saveSHPPath);
        }

        
        #endregion

        #region 导出图片
        private void ExportPictureBase(string FilePath)
        {

            if (!string.IsNullOrEmpty(FilePath))
            {
                var tool = new PictureThread(FilePath, axMapControl1.ActiveView,this.dataType,this.MapType);
                var thread = new Thread(tool.ThreadMain) { IsBackground = true };
                OperatorTxt.Text = "开始导出图片：" + FilePath;
                thread.Start();
                thread.Join();
                OperatorTxt.Text = "成功导出图片：" + FilePath;
              
            }
        }
        private void ExportBase()
        {
            var saveFilePath = FileHelper.Save("导出地图为图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png");
            this.Invoke(new EventOperator2(ExportPictureBase), new[] { saveFilePath });
        }
        private void btnExpImgRoad_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.Road;
            ExportBase();
        }
        private void ExportBusPicture_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.BusLine;
            ExportBase();
        }
        private void btnExpImgStop_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.Parking;
            ExportBase();
        }
        private void btnExpImgFlow_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.Flow;
            ExportBase();
        }
        private void btnExpImgBike_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.Bike;
            ExportBase();
        }
        /// <summary>
        /// 导出路网图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportActiveView_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出地图为图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            this.Invoke(new EventOperator2(ExportPictureBase), new[] { saveFilePath });
        }
       
        #endregion

        #region 导出Excel文件
        private void ExportExcelBase(IFeatureClass FeatureClass, string WhereClause, string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                OperatorTxt.Text = "开始导出Excel文件：" + FilePath;
                var HeadDict = GISHelper.GetFieldIndexDict(FeatureClass);
                ExcelHelper.SaveExcel(FeatureClass, WhereClause, FilePath, HeadDict);
                OperatorTxt.Text = "成功导出Excel文件：" + FilePath;
            }
        }
        private void ExportExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出路网属性表格", "2003 xls 文件|*.xls|2007 xlsx|*.xlsx");
            ExportExcelBase(RoadFeatureClass, RoadFilterWhereClause, saveFilePath);
        }
        private void ExportBusLineExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出公交车路线Excel文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(BusLineFeatureClass, BusLineWhereClause, saveFilePath);
        }
        private void btnExpXlsBusStop_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出公交车站点Excel文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(BusStopFeatureClass, BusStopWhereClause, saveFilePath);
        }
        private void ExportParkingExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出停车场属性文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(ParkingFeatureClass, ParkingWhereClause, saveFilePath);
        }
        private void ExportBikeExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出公共自行车属性文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(BikeFeatureClass, BikeWhereClause, saveFilePath);
        }
        private void ExportFlowExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出交通流量属性文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(FlowFeatureClass, FlowWhereClause, saveFilePath);
        }
        #endregion

        #region  地图点击 移动 双击

        private IFeatureLayer GetFeatureLayer(string LayerName)
        {
            IFeatureLayer featureLayer = null;
            for (var i = 0; i < axMapControl1.Map.LayerCount; i++)
            {
                if (axMapControl1.Map.get_Layer(i) is GroupLayer)
                {
                    ICompositeLayer compositeLayer = axMapControl1.Map.get_Layer(i) as ICompositeLayer;
                    for (var j = 0; j < compositeLayer.Count; j++)
                    {
                        featureLayer = compositeLayer.get_Layer(j) as IFeatureLayer;
                        if (featureLayer.Name == LayerName)
                        {
                            return featureLayer;
                        }
                    }
                }
                else
                {
                    featureLayer = axMapControl1.get_Layer(i) as IFeatureLayer;
                    if (featureLayer.Name == LayerName)
                    {
                        return featureLayer;
                    }
                }

            }
            return featureLayer;
        }
        private IFeatureLayer GetFeatureLayer(MapControl mapControl, string LayerName)
        {
            IFeatureLayer featureLayer = null;
            for (var i = 0; i < mapControl.Map.LayerCount; i++)
            {
                featureLayer = mapControl.get_Layer(i) as IFeatureLayer;
                if (featureLayer.Name == LayerName)
                {
                    return featureLayer;
                }
            }
            return featureLayer;
        }
        //移动
        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            lblCoords.Text = string.Format("{0:#.#####},{1:#.#####}", e.mapX, e.mapY);
        }
        #endregion

        #region  点选  路网 公交路线  公交站点  停车场
        public  void SelectFeature(IGeometry geometry,SpaceMode mode,IFeatureLayer FeatureLayer)
        {
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = geometry;
            switch (mode)
            {
                case SpaceMode.Contains:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case SpaceMode.Crossed:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case SpaceMode.Intersect:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
                case SpaceMode.Overlaps:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                    break;
                case SpaceMode.Touches:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                    break;
                case SpaceMode.Within:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    break;
            }
            
            short Opacity = 0;
            if (short.TryParse(System.Configuration.ConfigurationManager.AppSettings["OPACITY"], out Opacity))
            {
                ILayerEffects layerEffects = FeatureLayer as ILayerEffects;
                layerEffects.Transparency = Opacity;
            }
            IFeatureSelection featureSelection = FeatureLayer as IFeatureSelection;
            featureSelection.SelectFeatures((IQueryFilter)spatialFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);
            if (this.dataType == DataType.BusLine || this.dataType == DataType.BusStop)
            {
                featureSelection.SelectionColor = DisplayHelper.GetSelectRGBColor();
            }
            
            MapRefresh();
        }
        public  void AnalyzeBase(IGeometry geometry,SpaceMode mode,IFeatureLayer FeatureLayer,string Title)
        {
            SelectFeature(geometry, mode, FeatureLayer);
            var result = new AttributeForm2(FeatureLayer.FeatureClass, geometry, mode, Title,this.dataType);
            result.Show(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Feature">点击选中的道路</param>
        /// <param name="mode">空间关系</param>
        /// <param name="FeatureLayer1">公交路线 要素图层</param>
        /// <param name="Title">表名</param>
        /// <param name="FeatureLayer2">路网要素图层</param>
        private void AnalyzeBase(IFeature Feature, SpaceMode mode, IFeatureLayer FeatureLayer1,string Title,IFeatureLayer FeatureLayer2)
        {
            SelectFeature(Feature.Shape, mode, FeatureLayer1);
            var result = new AttributeForm2(FeatureLayer1.FeatureClass, Feature.Shape, mode, Title, this.dataType,FeatureLayer2.FeatureClass,Feature);
            result.Show(this);
        }
        public void Analyze(IGeometry geometry)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (TempGeometry != null)
                {
                    geometry = geometry.WMerge(TempGeometry);
                }
            }
            else
            {
                this.TempGeometry = null;
            }
            switch (this.dataType)
            {
                case DataType.BusLine:
                    //获取与公交路线相交
                    IFeatureLayer BusLinefeatureLayer = GetFeatureLayer(BusLineName.GetLayer());
                    AnalyzeBase(geometry, SpaceMode.Intersect, BusLinefeatureLayer, "当前区域内公交路线");
                    //获取 包含的公交站点
                    IFeatureLayer BusStopFeatureLayer = GetFeatureLayer(BusStopName.GetLayer());
                    AnalyzeBase(geometry, SpaceMode.Contains, BusStopFeatureLayer, "当前区域内公交站点");
                    break;
                case DataType.Parking:
                    IFeatureLayer ParkingFeatureLayer = GetFeatureLayer(ParkingName.GetLayer());
                    AnalyzeBase(geometry, SpaceMode.Contains, ParkingFeatureLayer, "当前区域内停车设施");
                    break;
            }

            TempGeometry = geometry;
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, axMapControl1.ActiveView.Extent);

        }
        private void Analyza(IFeature Feature)
        {
            IFeatureLayer RoadFeatureLayer = GetFeatureLayer(RoadName.GetLayer());
            IFeatureLayer BusLineFeatureLayer = GetFeatureLayer(BusLineName.GetLayer());
            AnalyzeBase(Feature, SpaceMode.Intersect, BusLineFeatureLayer, "经过道路上的公交路线", RoadFeatureLayer);
        }
        public void Analyze2(IGeometry geometry)
        {
            IFeatureLayer featureLayer = GetFeatureLayer(BusLineName.GetLayer());
            AnalyzeBase(geometry, SpaceMode.Intersect, featureLayer, "经过该道路公交路线");
        }
        public void Analyze2(IFeature Feature)
        {
            this.Invoke(new EventOperator3(Analyza), new[] { Feature });
        }
        private void ShowAttribute(IGeometry geometry)
        {
            IFeatureClass CurrentFeatureClass = null;
            string LayerName = string.Empty;
            string whereClause = string.Empty;
            switch (dataType)
            {
                case DataType.Road:
                    LayerName = RoadName;
                    CurrentFeatureClass = RoadFeatureClass;
                    whereClause = RoadFilterWhereClause;
                    break;
                case DataType.BusLine:
                    LayerName = BusLineName;
                    CurrentFeatureClass = BusLineFeatureClass;
                    whereClause = BusLineWhereClause;
                    break;
                case DataType.BusStop:
                    LayerName = BusStopName;
                    CurrentFeatureClass = BusStopFeatureClass;
                    whereClause = BusStopWhereClause;
                    break;
                case DataType.Parking:
                    LayerName = ParkingName;
                    CurrentFeatureClass = ParkingFeatureClass;
                    whereClause = ParkingWhereClause;
                    break;
                case DataType.Bike:
                    LayerName = BikeName;
                    CurrentFeatureClass = BikeFeatureClass;
                    whereClause = BikeWhereClause;
                    break;
                case DataType.Flow:
                    LayerName = FlowName;
                    CurrentFeatureClass = FlowFeatureClass;
                    whereClause = FlowWhereClause;
                    break;
            }
            
            IArray array = AttributeHelper.Identify(CurrentFeatureClass, geometry,whereClause);
            if (array != null)
            {
                IFeatureIdentifyObj featureIdentifyObj = array.get_Element(0) as IFeatureIdentifyObj;
                IIdentifyObj identifyObj = featureIdentifyObj as IIdentifyObj;
                IRowIdentifyObject rowidentifyObject = featureIdentifyObj as IRowIdentifyObject;
                IFeature feature = rowidentifyObject.Row as IFeature;
                //IFeature feature = AttributeHelper.Identify2(RoadFeatureClass, geometry);
                if (feature != null)
                {
                    Twinkle(feature);
                    AttributeForm form = new AttributeForm(feature, CurrentFeatureClass, LayerName);
                    form.ShowDialog(this);
                }
            }

        }

        private void ClickSearchBase(string LayerName, string WhereClause)
        {
            var cmd = new ClickSearchTool(LayerName,WhereClause, this.axMapControl1, this);
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        /// <summary>
        /// 点击 点选查询路网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointSearch_Click(object sender, EventArgs e)
        {
            ClickSearchBase(RoadName.GetLayer(), RoadFilterWhereClause);
        }
        //公交点选
        private void PointBusLine_Click(object sender, EventArgs e)
        {
            ClickSearchBase(BusLineName.GetLayer(), BusLineWhereClause);
        }

        //公交站点  点选
        private void PointBusStop_Click(object sender, EventArgs e)
        {
            ClickSearchBase(BusStopName.GetLayer(), BusStopWhereClause);
        }

        /// <summary>
        /// 停车场
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointParkingButton_Click(object sender, EventArgs e)
        {
            ClickSearchBase(ParkingName.GetLayer(), ParkingWhereClause);
        }
        private void PointBikeButton_Click(object sender, EventArgs e)
        {
            ClickSearchBase(BikeName.GetLayer(), BikeWhereClause);
        }

        private void PointFlowButton_Click(object sender, EventArgs e)
        {
            ClickSearchBase(FlowName.GetLayer(), FlowWhereClause);
        }
        #endregion

        #region  搜索  路网（条件）  公交（公交路线） 停车场

        private void FilterBase(DataType dataType, InquiryMode inquiryMode)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            this.dataType = dataType;
            this.inquiryMode = inquiryMode;
            IFeatureClass CurrentFeatureClass = null;
            switch (dataType)
            {
                case DataType.Road:
                    CurrentFeatureClass = RoadFeatureClass;
                    break;
                case DataType.Parking:
                    CurrentFeatureClass = ParkingFeatureClass;
                    break;
                case DataType.Bike:
                    CurrentFeatureClass = BikeFeatureClass;
                    break;
                case DataType.Flow:
                    CurrentFeatureClass = FlowFeatureClass;
                    break;
                case DataType.BusLine:
                    CurrentFeatureClass = BusLineFeatureClass;
                    break;
                case DataType.BusStop:
                    CurrentFeatureClass = BusStopFeatureClass;
                    break;
            }
            var filterform = new FilterForm(CurrentFeatureClass);
            filterform.ShowDialog(this);
        }
        /// <summary>
        ///  路网条件查询  显示表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionSearchButton_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Road, InquiryMode.Search);
        }
        private void SearchParkingButton_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Parking, InquiryMode.Search);
        }

        private void SearchBike_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Bike, InquiryMode.Search);

        }
        private void SearchFlow_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Flow, InquiryMode.Search);
        }

        private void BusLineSearch2_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusLine, InquiryMode.Search);
        }

        private void BusStopSearch2_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusStop, InquiryMode.Search);
        }
        #endregion

        #region  过滤  路网、公共自行车
        

        /// <summary>
        /// 过滤路网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoadFilter_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Road,InquiryMode.Filter);
        }
        private void btnFilterParking_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Parking, InquiryMode.Filter);  
        }

        private void BikeFilter_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Bike,InquiryMode.Filter);
        }
        private void FlowFlter_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Flow, InquiryMode.Filter);
        }
        #endregion

        #region 统计
        private void StatisticParkingButton_Click(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region  交通流量监视器操作
        private void Operate(IGeometry geometry)
        {
            IFeatureClass currentFeatureClass = null;
            string whereClause = string.Empty;
            string Warning = string.Empty;
            switch (this.dataType)
            {
                case DataType.Flow:
                    currentFeatureClass = FlowFeatureClass;
                    whereClause = FlowWhereClause;
                    Warning = FlowName.GetLayer();
                    break;
                case DataType.Parking:
                    currentFeatureClass = ParkingFeatureClass;
                    whereClause = ParkingWhereClause;
                    Warning = ParkingName.GetLayer();
                    break;
                case DataType.Road:
                    currentFeatureClass = RoadFeatureClass;
                    whereClause = RoadFilterWhereClause;
                    Warning = RoadName.GetLayer();
                    break;
            }
            
            switch (this.operateMode)
            {
                case OperateMode.Add:
                    OperateForm operateform = new OperateForm(currentFeatureClass,geometry);
                    operateform.ShowDialog(this);
                    break;
                case OperateMode.Delete:
                     IArray array = AttributeHelper.Identify(currentFeatureClass, geometry,whereClause);
                     if (array != null)
                     {
                         IFeatureIdentifyObj featureIdentifyObj = array.get_Element(0) as IFeatureIdentifyObj;
                         IIdentifyObj identifyObj = featureIdentifyObj as IIdentifyObj;
                         IRowIdentifyObject rowidentifyObject = featureIdentifyObj as IRowIdentifyObject;
                         IFeature feature = rowidentifyObject.Row as IFeature;
                         if (feature != null)
                         {
                             Twinkle(feature);
                             if (MessageBox.Show("您确定要删除当前选择"+Warning, "警告",MessageBoxButtons.OKCancel) == DialogResult.OK)
                             {
                                 feature.Delete();
                                 MapRefresh();
                             }
                         }
                     }
                    break;
                case OperateMode.Edit:
                     IArray arrayEdit = AttributeHelper.Identify(currentFeatureClass, geometry,whereClause);
                     if (arrayEdit != null)
                     {
                         IFeatureIdentifyObj featureIdentifyObj = arrayEdit.get_Element(0) as IFeatureIdentifyObj;
                         IIdentifyObj identifyObj = featureIdentifyObj as IIdentifyObj;
                         IRowIdentifyObject rowidentifyObject = featureIdentifyObj as IRowIdentifyObject;
                         IFeature feature = rowidentifyObject.Row as IFeature;
                         if (feature != null)
                         {
                             Twinkle(feature);
                             var form = new OperateForm(currentFeatureClass, feature);
                             form.ShowDialog(this);
                         }
                     }
                    break;
            }
            this.operateMode = OperateMode.None;
            
        }
        
        #endregion

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            AddUserForm addform = new AddUserForm();
            addform.ShowDialog();
        }

        #region 停车场 操作
        
        #endregion

        private void UserList_Click(object sender, EventArgs e)
        {
            UserForm form = new UserForm();
            form.ShowDialog();
        }
        public void StatisticBase(string Description)
        {
            StatisticMode mode = Description.GetEnum();
            var dict = GISHelper.Statistic(RoadFeatureClass, mode);
            StatisticsForm statisticsForm = new StatisticsForm(dict, Description+"路网总长度统计图");
            statisticsForm.ShowDialog();
            
        }
        //路网统计
        private void RoadStatistic_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            foreach (StatisticMode mode in Enum.GetValues(typeof(StatisticMode)))
            {
                list.Add(mode.GetDescription());
            }
            this.inquiryMode = InquiryMode.Statistic;
            SelectForm selectform = new SelectForm(list);
            selectform.ShowDialog(this);
        }
        //公交路线过滤
        private void BusFilter_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusLine, InquiryMode.Filter);
        }
        //公交路线 区域过滤
        private void RegionFilter_Click(object sender, EventArgs e)
        {
            this.TempGeometry = null;
            this.dataType = DataType.BusLine;
            var cmd = new FrameSearchTool(axMapControl1, this,BusLineName.GetLayer());
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
            //axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            
        }

        #region 渲染更改  路网  公交
        private void Romance(string LayerName, string RendererName)
        {
            IFeatureLayer CurrentFeatureLayer = GetFeatureLayer(LayerName);
            IGeoFeatureLayer CurrentGeoFeatureLayer = CurrentFeatureLayer as IGeoFeatureLayer;
            CurrentGeoFeatureLayer.Renderer = RendererHelper.GetRenderer(this.MapControl, RendererName);
            axTOCControl1.Refresh();
            axTOCControl1.Update();
            axMapControl1.Refresh();
            axMapControl1.Update();
        }
        //路网等级图
        private void RankMap_Click(object sender, EventArgs e)
        {
            this.MapType = "Rank";
            Romance(RoadName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["RoadRank"]);
        }
        //路网车道数图
        private void NumMap_Click(object sender, EventArgs e)
        {
            this.MapType = "Num";
            Romance(RoadName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["RoadNumber"]);
        }
        //路网基础图
        private void RoadBaseMap_Click(object sender, EventArgs e)
        {
            this.MapType = "Base";
            Romance(RoadName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["RoadBase"]);
        }
        //公交等级图
        private void BusDegree_Click(object sender, EventArgs e)
        {
            this.MapType = "Rank";
            Romance(BusLineName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["BusDegree"]);
        }
        //公交区域图
        private void BusRegion_Click(object sender, EventArgs e)
        {
            this.MapType = "Region";
            Romance(BusLineName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["BusRegion"]);
        }
        //公交基础图
        private void BusLineBaseMap_Click(object sender, EventArgs e)
        {
            this.MapType = "Base";
            Romance(BusLineName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["BusBase"]);
        } 
        //进行唯一值渲染
        private void UniqueValueRenderer(IFeatureLayer featureLayer, string FieldName)
        {
            int FieldIndex = featureLayer.FeatureClass.Fields.FindField(FieldName);
            IUniqueValueRenderer uniqueValueRenderer = new UniqueValueRendererClass();
            uniqueValueRenderer.FieldCount = 1;
            uniqueValueRenderer.set_Field(0, FieldName);
            IRandomColorRamp randomColorRamp = new RandomColorRampClass();
            randomColorRamp.StartHue = 0;
            randomColorRamp.MinValue = 0;
            randomColorRamp.MinSaturation = 15;
            randomColorRamp.EndHue = 360;
            randomColorRamp.MaxValue = 100;
            randomColorRamp.MaxSaturation = 30;
            IQueryFilter queryFilter = new QueryFilterClass();
            randomColorRamp.Size = featureLayer.FeatureClass.FeatureCount(queryFilter);
            bool flag = false;
            randomColorRamp.CreateRamp(out flag);
            IEnumColors enumColors = randomColorRamp.Colors;
            IColor color = null;
            object codeValue = null;
            queryFilter = new QueryFilterClass();
            queryFilter.AddField(FieldName);
            IFeatureCursor featureCursor= featureLayer.FeatureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                codeValue = feature.get_Value(FieldIndex);
                color = enumColors.Next();
                if (color == null)
                {
                    enumColors.Reset();
                    color = enumColors.Next();
                }
                ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Color = color;
                uniqueValueRenderer.AddValue(codeValue.ToString(), "", simpleLineSymbol as ISymbol);
                feature = featureCursor.NextFeature();
            }
            IGeoFeatureLayer geoFeatureLayer = featureLayer as IGeoFeatureLayer;
            geoFeatureLayer.Renderer = uniqueValueRenderer as IFeatureRenderer;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            axMapControl1.Refresh();
            axMapControl1.Update();
        }
        #endregion

        //停车设施  区域过滤
        private void btnRegionFilterParking_Click(object sender, EventArgs e)
        {
            this.TempGeometry = null;
            this.dataType = DataType.Parking;
            var cmd = new FrameSearchTool(axMapControl1, this, ParkingName.GetLayer());
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }

        #region 添加  编辑  删除  操作

        private void Operate(string LayerName,string WhereClause)
        {
            var cmd = new ClickSearchTool(LayerName, WhereClause, axMapControl1, this);
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        // 路网  编辑
        private void RoadEdit_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Road;
            Operate(RoadName.GetLayer(), RoadFilterWhereClause);
            
        }

        private void RoadDelete_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Road;
            Operate(RoadName.GetLayer(), RoadFilterWhereClause);
        }

        private void btnAddRoad_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.None;
            var dialog = new OpenFileDialog { Title = "请选择需要导入的CAD文件", Filter = "DXF文件 (*.dxf)|*.dxf" };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

           
            var factory = new CadWorkspaceFactory();
            try
            {
                var path = System.IO.Path.GetDirectoryName(dialog.FileName);
                var ws = factory.OpenFromFile(path, 0) as IFeatureWorkspace;

                var fc = ws.OpenFeatureClass(System.IO.Path.GetFileName(dialog.FileName) + ":polyline");

                var cursor = fc.Search(null, true);
                var f = cursor.NextFeature();

                var lst = new List<IPolyline>();
                while(f!= null)
                {
                    var geo = f.ShapeCopy;
                    if (!(geo is IPolyline) || geo.IsEmpty == true)
                    {
                        MessageBox.Show("当前CAD文件中包含的路线类型信息不正确，请检查。", "注意");
                        Marshal.ReleaseComObject(cursor);
                        return;
                    }
                    else
                    {
                        lst.Add(geo as IPolyline);
                    }
                    f = cursor.NextFeature();
                }
                Marshal.ReleaseComObject(cursor);

                
                if (lst.Count == 0)
                {
                    MessageBox.Show("当前CAD文件中不包含路线信息，请检查。", "注意");
                    return;
                }

                ImportRoad(lst);
            }
            catch(Exception ex)
            {
                MessageBox.Show("打开CAD文件错误，请检查CAD文件。\r\n详情：" + ex, "注意");
            }
        }

        private void ImportRoad(List<IPolyline> pls)
        {
            var fc = RoadFeatureClass;
            var historyFC = RoadHistoryFeatureClass;
            var crossFC = RoadNodeFeatureClass;
            RoadMerger.FragmentThreshold = 20;
            var ls = RoadMerger.SplitLine(pls);
            m_ImportRoads.Clear();
            m_ImportRoads.AddRange(ls);
            var lines = RoadMerger.QueryIntersects(ls, fc);
            m_Crossroads.Clear();
            foreach(var line in lines)
            {
                m_Crossroads.AddRange(line.Crossings.Select(x=>x.Crossing));
            }

            /*
            var ret = RoadHelper.QueryIntersectPoints(pls, RoadFeatureClass);

            var list = ret.Where(x => x.Value == null).ToList();

            if (list.Count > 0)
            {
                var f = fc.GetFeature(list[0].Key);
                MessageBox.Show(string.Format("导入的路线与已有线路'{0}({1})'部分重合，无法完成导入", f.get_Value(f.Fields.FindField("NAME")), f.get_Value(f.Fields.FindField("NO_"))), "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conditions = ret.Select(x => string.Format("{0} = {1}", RoadFeatureClass.OIDFieldName, x.Key));
            var fl = GetFeatureLayer(RoadName.GetLayer());

            // 选中图形
            axMapControl1.Map.ClearSelection();
            var featureSelection = fl as IFeatureSelection;
            featureSelection.SelectFeatures(
                new QueryFilter { WhereClause = string.Join(" OR ", conditions.ToArray()) },
                esriSelectionResultEnum.esriSelectionResultNew,
                false);

            // 将交点也加入显示
            m_Crossroads.Clear();
            m_Crossroads.AddRange(TransformToCrossroadInfo(ret, fc));

            m_ImportRoads = pl;

            Center(pl);*/

            axMapControl1.Refresh(esriViewDrawPhase.esriViewForeground, Type.Missing, Type.Missing);
            //axMapControl1.ActiveView.Refresh();

            var form = new ImportRoadForm(ls, fc, historyFC, crossFC, lines);
            form.Show(this);
        }

        private void AddParking_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Add;
            this.dataType = DataType.Parking;
            Operate(ParkingName.GetLayer(), ParkingWhereClause);
        }
        private void DeleteParking_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Parking;
            Operate(ParkingName.GetLayer(), ParkingWhereClause);
        }

        private void EditParking_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Parking;
            Operate(ParkingName.GetLayer(), ParkingWhereClause);
        }

        private void AddFlowPoint_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Add;
            this.dataType = DataType.Flow;
            Operate(FlowName.GetLayer(), FlowWhereClause);
        }

        private void DeleteFlowPoint_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Flow;
            Operate(FlowName.GetLayer(), FlowWhereClause);
        }

        private void EditFlowPoint_Click(object sender, EventArgs e)
        {
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Flow;
            Operate(FlowName.GetLayer(), FlowWhereClause);
        }

        #endregion
        

        public void EraseImportRoadCustomDrawing()
        {
            m_Crossroads.Clear();
            m_ImportRoads = null;
            axMapControl1.ActiveView.Refresh();
        }

        private static List<CrossroadInfo> TransformToCrossroadInfo(Dictionary<int, List<IPoint>> dict, IFeatureClass fc)
        {
            var ret = new List<CrossroadInfo>();
            foreach(var pair in dict)
            {
                var f = fc.GetFeature(pair.Key);
                
                var name = f.get_Value(f.Fields.FindField("NAME")).ToString();
                var no = Convert.ToInt32(f.get_Value(f.Fields.FindField("NO_")));

                foreach(var pt in pair.Value)
                {
                    ret.Add(new CrossroadInfo{
                        Name=name,
                        NO=no,
                        Point=pt,
                        OID=pair.Key
                    });
                }
            }
            return ret;
        }

        

        private void BtnRoadBus_Click(object sender, EventArgs e)
        {
            var RoadFeatureLayer = GetFeatureLayer(RoadName.GetLayer());
            var BusLineFeatureLayer = GetFeatureLayer(BusLineName.GetLayer());
            if (RoadFeatureLayer.Visible == false||BusLineFeatureLayer.Visible==false)
            {
                RoadFeatureLayer.Visible = true;
                BusLineFeatureLayer.Visible = true;
                MapRefresh();
            }
            var cmd = new ClickSearchTool(RoadName.GetLayer(), RoadFilterWhereClause, this.axMapControl1, this, true);
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }

        private void btnBusLineNumber_Click(object sender, EventArgs e)
        {
            var list = GISHelper.GetUniqueValue(BusLineFeatureClass,System.Configuration.ConfigurationManager.AppSettings["BUSKEY"]);
            BusFilterForm form = new BusFilterForm(list,BusLineFeatureClass);
            form.ShowDialog(this);
        }

        private void PeoplePostBase(string Description)
        {
            ILayer layer = null;
            IFeatureLayer featureLayer = null;
            string Ignore = System.Configuration.ConfigurationManager.AppSettings["Ignore"];
            string AddLayer = System.Configuration.ConfigurationManager.AppSettings["RoadBackground"];
            ICompositeLayer compositeLayer = null;
            for (var i = 0; i < axMapControl1.Map.LayerCount; i++)
            {
                layer = axMapControl1.Map.get_Layer(i);
                if (layer.Name == Ignore)
                {
                    continue;
                }
                if (layer is GroupLayer)
                {
                    compositeLayer = layer as ICompositeLayer;
                    for (var j = 0; j < compositeLayer.Count; j++)
                    {
                        featureLayer = compositeLayer.get_Layer(j) as IFeatureLayer;
                        if (featureLayer.Name == Description)
                        {
                            featureLayer.Visible = true;
                        }
                        else
                        {
                            featureLayer.Visible = false;
                        }
                    }
                }
                else if (layer is FeatureLayer)
                {
                    featureLayer = layer as IFeatureLayer;
                    if (featureLayer.Name == Description||featureLayer.Name==AddLayer)
                    {
                        featureLayer.Visible = true;
                    }
                    else
                    {
                        featureLayer.Visible = false;
                    }
                }
            }
            if (ExtentFeature != null)
            {
                axMapControl1.Extent = ExtentFeature.Shape.Envelope;
            }
            axMapControl1.ActiveView.Refresh();
        }

        private void CurrentPeople_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.CurrentPeople.GetDescription());
        }

        private void CurrentPeopleDensity_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.CurrentPostDensity.GetDescription());
        }

        private void PlanPeople_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.PlanPeople.GetDescription());
        }

        private void PlanPeopleDensity_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.PlanPeopleDensity.GetDescription());
        }

        private void CurrentPost_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.CurrentPost.GetDescription());
        }

        private void CurrentPostDensity_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.CurrentPostDensity.GetDescription());
        }

        private void PlanPost_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.PlanPost.GetDescription());
        }

        private void PlanPostDensity_Click(object sender, EventArgs e)
        {
            PeoplePostBase(PeoplePost.PlanPostDensity.GetDescription());
        }

        #region 取消过滤
        private void CanelRoadFilter_Click(object sender, EventArgs e)
        {
            UpdateBase(RoadName, "", RoadFeatureClass);
            this.RoadFilterWhereClause = "";
        }

        private void CancelBusFilter_Click(object sender, EventArgs e)
        {
            UpdateBase(BusLineName, "", BusLineFeatureClass);
            UpdateBase(BusStopName, "", BusStopFeatureClass);
            OpenClose(StartEndName, false);
            this.BusLineWhereClause = "";
            this.BusStopWhereClause = "";
        }

        private void CancelParkingFilter_Click(object sender, EventArgs e)
        {
            UpdateBase(ParkingName, "", ParkingFeatureClass);
            this.ParkingWhereClause = "";
        }

        private void CancelBikeFilter_Click(object sender, EventArgs e)
        {
            UpdateBase(BikeName, "", BikeFeatureClass);
            this.BikeWhereClause = "";
        }

        private void CancelFlowFilter_Click(object sender, EventArgs e)
        {
            UpdateBase(FlowName, "", FlowFeatureClass);
            this.FlowWhereClause = "";
        }
        #endregion

        private void SaveInit(object sender)
        {
            this.operateMode = OperateMode.None;
            UncheckAllButtons(sender);
            var cmd = new ControlsMapPanTool();
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }
        private void RoadSave_Click(object sender, EventArgs e)
        {
            SaveInit(sender);
        }

        //停车场  行政区统计
        private void XZQStatistic_Click(object sender, EventArgs e)
        {
            var dict = GISHelper.Statistic(ParkingFeatureClass, "ZHENGQU", "BERTHNUM");
            StatisticsForm statisticform = new StatisticsForm(dict, "");
            statisticform.ShowDialog();
        }

        //停车场  框选统计
        private void RegionStatistic_Click(object sender, EventArgs e)
        {
            this.TempGeometry = null;
            this.dataType = DataType.Parking;
            var cmd = new FrameSearchTool(axMapControl1, this, ParkingName.GetLayer(),true);
            cmd.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ITool)cmd;
        }

        
        public bool IsCtrl()
        {
            return ((Control.ModifierKeys & Keys.Control) == Keys.Control);
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {

        }
    }
    
   
}
