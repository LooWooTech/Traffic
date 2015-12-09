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
    public partial class MainForm : Form
    {
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
        #region  初始化
        private string MXDPath { get; set; }
        private string RoadName { get; set; }
        private string BusLineName { get; set; }
        private string BusStopName { get; set; }
        private string ParkingName { get; set; }
        private string BikeName { get; set; }
        private string FlowName { get; set; }
        private string XZQName { get; set; }
        public  string RoadFilterWhereClause { get; set; }
        public string BusLineWhereClause { get; set; }
        public string BusStopWhereClause { get; set; }
        public string ParkingWhereClause { get; set; }
        public string BikeWhereClause { get; set; }
        public string FlowWhereClause { get; set; }
        private IFeatureClass RoadFeatureClass { get; set; }
        private IFeatureClass BusLineFeatureClass { get; set; }
        private IFeatureClass BusStopFeatureClass { get; set; }
        private IFeatureClass ParkingFeatureClass { get; set; }
        private IFeatureClass BikeFeatureClass { get; set; }
        private IFeatureClass FlowFeatureClass { get; set; }
        private IFeatureClass XZQFeatureClass { get; set; }
        private INewPolygonFeedback newPolygonFeedback { get; set; }
        private MapControl MapControl { get; set; }
        private bool RoadFlag { get; set; }
        public  InquiryMode inquiryMode { get; set; }
        public DataType dataType { get; set; }
        public OperateMode operateMode { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        public User CurrentUser { get; set; }
        public SplashForm Splash { get; set; }

        private readonly List<CrossroadInfo> m_Crossroads = new List<CrossroadInfo>();
        private IMarkerSymbol m_CrossroadSymbol;
        private IPolyline m_ImportRoad;
        private ILineSymbol m_ImportRoadSymbol;

        public MainForm()
        {
            InitializeComponent();
            MapControl = RendererHelper.GetMapControl();
            MXDPath = System.Configuration.ConfigurationManager.AppSettings["MXD"];
            RoadName = System.Configuration.ConfigurationManager.AppSettings["ROAD"];
            BusLineName = System.Configuration.ConfigurationManager.AppSettings["BUSLINE"];
            BusStopName = System.Configuration.ConfigurationManager.AppSettings["BUSSTOP"];
            ParkingName = System.Configuration.ConfigurationManager.AppSettings["PARKING"];
            BikeName = System.Configuration.ConfigurationManager.AppSettings["BIKE"];
            FlowName = System.Configuration.ConfigurationManager.AppSettings["FLOW"];
            XZQName = System.Configuration.ConfigurationManager.AppSettings["XZQ"];
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
            BusLineFeatureClass = SDEManager.GetFeatureClass(BusLineName);
            BusStopFeatureClass = SDEManager.GetFeatureClass(BusStopName);
            ParkingFeatureClass = SDEManager.GetFeatureClass(ParkingName);
            BikeFeatureClass = SDEManager.GetFeatureClass(BikeName);
            FlowFeatureClass = SDEManager.GetFeatureClass(FlowName);
            XZQFeatureClass = SDEManager.GetFeatureClass(XZQName);
            if (Splash != null)
            {
                Splash.panel1.Visible = true;
                Splash.progressBar1.Visible = false;
                this.Enabled = false;
            }
        }
        #endregion

        #region  地图显示更新
        public void ConditionControlCenter()
        {
            string WhereClause = string.Empty;
            string LayerName = string.Empty;
            IFeatureClass CurrentFeatureClass = null;
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
                    break;
                case DataType.BusStop:
                    BusStopWhereClause = toolStripStatusLabel1.Text;
                    break;
            }
            if (this.dataType == DataType.BusLine)
            {
                var list = GISHelper.Search(CurrentFeatureClass, WhereClause);
                BusStopWhereClause = GISHelper.GetBusStopWhereClause(list,  CurrentFeatureClass.Fields.FindField("ShortName"), CurrentFeatureClass.Fields.FindField("lineDirect"));
                switch (this.inquiryMode)
                {
                    case InquiryMode.Filter:
                        UpdateBase(BusStopName, BusStopWhereClause, BusStopFeatureClass);
                        break;
                
                }
            }
            switch (this.inquiryMode)
            {
                case InquiryMode.Filter://
                    UpdateBase(LayerName, WhereClause,CurrentFeatureClass);
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
        private void UpdateBase(string Name,string WhereClause,IFeatureClass FeatureClass)
        {
            var CurrentLayerName = Name.GetLayer();
            IFeatureLayer featureLayer = GetFeatureLayer(CurrentLayerName);
            if (featureLayer != null)
            {
                IFeatureLayerDefinition featureLayerDefinition = featureLayer as IFeatureLayerDefinition;
                featureLayerDefinition.DefinitionExpression = WhereClause;
                Center(FeatureClass, WhereClause);
                axMapControl1.ActiveView.Refresh();
            }
            

          
            
        }
        ///// <summary>
        ///// 根据查询条件 更新路网数据
        ///// </summary>
        //public void UpdateRoad()
        //{
        //    if (toolStripStatusLabel1.Text != RoadFilterWhereClause)
        //    {
        //        RoadFilterWhereClause = toolStripStatusLabel1.Text;
        //        UpdateBase(RoadName, RoadFilterWhereClause);
        //    }
        //}
        public void UpdateBus()
        {
            switch (dataType)
            {
                case DataType.BusLine:
                    //UpdateBase(BusLineName, BusLineWhereClause);
                    //UpdateBase(BusStopName, BusStopWhereClause);
                    break;
                case DataType.BusStop:
                    break;
                default:
                    break;
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
                CenterBase(Feature.Shape.Envelope);
            }
        }

        public void Center(IGeometry geo)
        {
            if(geo != null)
            {
                CenterBase(geo.Envelope);
            }
        }
        /// <summary>
        /// 居中
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <param name="WhereClause"></param>
        public void Center(IFeatureClass FeatureClass, string WhereClause)
        {
            IQueryFilter queryfilter = new QueryFilterClass();
            queryfilter.WhereClause = WhereClause;
            IFeatureCursor featurecursor = FeatureClass.Search(queryfilter, false);
            IFeature feature = featurecursor.NextFeature();
            IEnvelope envelope = new Envelope() as IEnvelope;
            while (feature != null)
            {
                envelope.Union(feature.Extent);
                feature = featurecursor.NextFeature();
            }
            CenterBase(envelope);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featurecursor);
        }
        #endregion
        public void ShowBus()
        {
            var result = new BusResultForm(BusLineFeatureClass,BusStopFeatureClass, BusLineWhereClause);
            result.Show(this);
        }
        private void ImportBusExcel_Click(object sender, EventArgs e)
        {
            var ExcelPath = FileHelper.Open("打开公交路线数据", "2003 文件|*.xls|2007 文件|*.xlsx");
            var list = ExcelHelper.Read(ExcelPath);
            var tool=new BusLineManager();
           
            try
            {
                tool.Add(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            OperatorTxt.Text = "导入公交路线信息成功";
            
        }
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
                var tool = new PictureThread(FilePath, axMapControl1.ActiveView);
                var thread = new Thread(tool.ThreadMain);
                thread.Start();
                //FileHelper.ExportMap(FilePath, axMapControl1.ActiveView);
                //OperatorTxt.Text = "成功导出图片："+FilePath;
            }
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
           // ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
        }
       
        #endregion

        #region 导出Excel文件
        private void ExportExcelBase(IFeatureClass FeatureClass, string WhereClause, string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
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
        //点击
        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            IGeometry geometry = point as IGeometry;
            if (axMapControl1.MousePointer == esriControlsMousePointer.esriPointerIdentify)
            {
                this.Invoke(new EventOperator(ShowAttribute), new[] { geometry });
            }
            else if (axMapControl1.MousePointer == esriControlsMousePointer.esriPointerArrow)
            {
                this.Invoke(new EventOperator(Operate), new[] { geometry });
            }
            else if (axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair)
            {
                if (newPolygonFeedback == null)
                {
                    axMapControl1.Map.ClearSelection();
                    newPolygonFeedback = new NewPolygonFeedbackClass();
                    newPolygonFeedback.Display = axMapControl1.ActiveView.ScreenDisplay;
                    newPolygonFeedback.Start(point);
                }
                else
                {
                    newPolygonFeedback.AddPoint(point);
                }
            }
        }
        //移动
        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            lblCoords.Text = string.Format("{0:#.#####},{1:#.#####}", e.mapX, e.mapY);
            if (axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair)
            {
                IPoint point = new PointClass();
                point.PutCoords(e.mapX, e.mapY);
                if (newPolygonFeedback != null)
                {
                    newPolygonFeedback.MoveTo(point);
                }
            }
        }
        //双击
        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair)
            {
                IPoint point = new PointClass();
                point.PutCoords(e.mapX, e.mapY);
                newPolygonFeedback.AddPoint(point);
                IPolygon polygon = newPolygonFeedback.Stop();
                newPolygonFeedback = null;
                this.Invoke(new EventOperator(Analyze), new[] { polygon as IGeometry });
            }
        }
        #endregion

        #region  点选  路网 公交路线  公交站点  停车场
        private void AnalyzeBase(IGeometry geometry,SpaceMode mode,IFeatureLayer FeatureLayer,string Title)
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
            IFeatureSelection featureSelection = FeatureLayer as IFeatureSelection;
            featureSelection.SelectFeatures((IQueryFilter)spatialFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);
            var result = new AttributeForm2(FeatureLayer.FeatureClass, geometry, mode, Title,this.dataType);
            result.Show(this);

        }
        private void Analyze(IGeometry geometry)
        {
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
            

            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, axMapControl1.ActiveView.Extent);

        }
        public void Analyze2(IGeometry geometry)
        {
            IFeatureLayer featureLayer = GetFeatureLayer(BusLineName.GetLayer());
            AnalyzeBase(geometry, SpaceMode.Intersect, featureLayer, "经过该道路公交路线");
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
            FilterBase(DataType.BusLine, InquiryMode.Filter);
        }

        private void BusStopSearch2_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusStop, InquiryMode.Filter);
        }
        /// <summary>
        /// 公交车路线搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBusLineButton_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusLine, InquiryMode.Search);
            //this.dataType = DataType.BusLine;
            //BusFilterForm busform = new BusFilterForm();
            //busform.ShowDialog(this);
        }
        /// <summary>
        /// 公交车站点搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBusStopButton_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusStop, InquiryMode.Search);
            //this.dataType = DataType.BusStop;
            //BusFilterForm busform = new BusFilterForm();
            //busform.ShowDialog(this);
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
            var dict = GISHelper.Statistic(ParkingFeatureClass, "ZHENGQU", "BERTHNUM");
            StatisticsForm statisticform = new StatisticsForm(dict,"");
            statisticform.ShowDialog();
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
        private void AddFlowPoint_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Add;
            this.dataType = DataType.Flow;
        }

        private void DeleteFlowPoint_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Flow;
        }

        private void EditFlowPoint_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Flow;
        }
        #endregion

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            AddUserForm addform = new AddUserForm();
            addform.ShowDialog();
        }

        #region 停车场 操作
        private void AddParking_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Add;
            this.dataType = DataType.Parking;
        }
        private void DeleteParking_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Parking;
        }

        private void EditParking_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Parking;
        }
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
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            this.dataType = DataType.BusLine;
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
            Romance(RoadName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["RoadRank"]);
        }
        //路网车道数图
        private void NumMap_Click(object sender, EventArgs e)
        {
            Romance(RoadName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["RoadNumber"]);
        }
        //路网基础图
        private void RoadBaseMap_Click(object sender, EventArgs e)
        {
            Romance(RoadName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["RoadBase"]);
        }
        //公交等级图
        private void BusDegree_Click(object sender, EventArgs e)
        {
            Romance(BusLineName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["BusDegree"]);
        }
        //公交区域图
        private void BusRegion_Click(object sender, EventArgs e)
        {
            Romance(BusLineName.GetLayer(), System.Configuration.ConfigurationManager.AppSettings["BusRegion"]);
        }
        //公交基础图
        private void BusLineBaseMap_Click(object sender, EventArgs e)
        {
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
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            this.dataType = DataType.Parking;
        }

        // 路网  编辑
        private void RoadEdit_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Road;
        }

        private void RoadDelete_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Road;
        }

        private void btnAddRoad_Click(object sender, EventArgs e)
        {
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
                if (f == null)
                {
                    MessageBox.Show("当前CAD文件中不包含路线信息，请检查。", "注意");
                    return;
                }

                var geo = f.ShapeCopy;
                if (!(geo is IPolyline) || geo.IsEmpty == true)
                {
                    MessageBox.Show("当前CAD文件中包含的路线信息不正确，请检查。", "注意");
                    Marshal.ReleaseComObject(cursor);
                    return;
                }

                f = cursor.NextFeature();
                if (f != null)
                {
                    if (MessageBox.Show("当前CAD文件中包含不止一条路线，系统默认导入第一条，是否继续？", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        Marshal.ReleaseComObject(cursor);
                        return;
                    }
                }

                ImportRoad(geo as IPolyline);
            }
            catch(Exception ex)
            {
                MessageBox.Show("打开CAD文件错误，请检查CAD文件。\r\n详情：" + ex, "注意");
            }
        }

        private void ImportRoad(IPolyline pl)
        {
            var fc = RoadFeatureClass;
            var ret = RoadHelper.QueryIntersectPoints(pl, RoadFeatureClass);

            var list = ret.Where(x => x.Value == null).ToList();

            if (list.Count > 0)
            {
                var f = fc.GetFeature(list[0].Key);
                MessageBox.Show(string.Format("导入的路线与已有线路'{0}({1})'部分重合，无法完成导入", f.get_Value(f.Fields.FindField("NAME")), f.get_Value(f.Fields.FindField("NO_"))), "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conditions = ret.Select(x=>string.Format("{0} = {1}", RoadFeatureClass.OIDFieldName, x.Key));
            var fl = GetFeatureLayer(RoadName.GetLayer());

            // 选中图形
            axMapControl1.Map.ClearSelection();
            var featureSelection = fl as IFeatureSelection;
            featureSelection.SelectFeatures(
                new QueryFilter{ WhereClause=string.Join(" OR ", conditions.ToArray())}, 
                esriSelectionResultEnum.esriSelectionResultNew, 
                false);
            
            // 将交点也加入显示
            m_Crossroads.Clear();
            m_Crossroads.AddRange(TransformToCrossroadInfo(ret, fc));

            m_ImportRoad = pl;

            Center(pl);
            axMapControl1.Refresh(esriViewDrawPhase.esriViewForeground, Type.Missing, Type.Missing);
            //axMapControl1.ActiveView.Refresh();

            var form = new ImportRoadForm(pl, fc, XZQFeatureClass, m_Crossroads);
            form.Show(this);
        }

        public void EraseImportRoadCustomDrawing()
        {
            m_Crossroads.Clear();
            m_ImportRoad = null;
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

        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            var viewDrawPhase = (esriViewDrawPhase)e.viewDrawPhase;
            //if (viewDrawPhase == esriViewDrawPhase.esriViewForeground)
            {

                object o = m_ImportRoadSymbol;
                if (m_ImportRoad != null)
                {
                    axMapControl1.DrawShape(m_ImportRoad, ref o);
                }

                object r = m_CrossroadSymbol;
                foreach (var info in m_Crossroads)
                {
                    axMapControl1.DrawShape(info.Point, ref r);
                }

                
            }
        }
    }
    
   
}
