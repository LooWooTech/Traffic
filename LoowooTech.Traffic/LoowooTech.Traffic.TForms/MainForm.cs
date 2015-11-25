using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.Manager;
using LoowooTech.Traffic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Traffic.TForms
{
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
        private bool RoadFlag { get; set; }
        public  InquiryMode inquiryMode { get; set; }
        public DataType dataType { get; set; }
        public OperateMode operateMode { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        private User CurrentUser { get; set; }
        public SplashForm Splash { get; set; }
        public MainForm()
        {
            InitializeComponent();
            MXDPath = System.Configuration.ConfigurationManager.AppSettings["MXD"];
            RoadName = System.Configuration.ConfigurationManager.AppSettings["ROAD"];
            BusLineName = System.Configuration.ConfigurationManager.AppSettings["BUSLINE"];
            BusStopName = System.Configuration.ConfigurationManager.AppSettings["BUSSTOP"];
            ParkingName = System.Configuration.ConfigurationManager.AppSettings["PARKING"];
            BikeName = System.Configuration.ConfigurationManager.AppSettings["BIKE"];
            FlowName = System.Configuration.ConfigurationManager.AppSettings["FLOW"];
            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 99);
            simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            simpleMarkerSymbol.Size = 8;
            simpleMarkerSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 0);

            try
            {
                axMapControl1.LoadMxFile(System.IO.Path.Combine(Application.StartupPath, MXDPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show("载入地图错误:" + ex.ToString());
            }

            axTOCControl1.SetBuddyControl(axMapControl1);
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            /*RoadFeatureClass = SDEManager.GetFeatureClass(RoadName);
            BusLineFeatureClass = SDEManager.GetFeatureClass(BusLineName);
            BusStopFeatureClass = SDEManager.GetFeatureClass(BusStopName);
            ParkingFeatureClass = SDEManager.GetFeatureClass(ParkingName);
            BikeFeatureClass = SDEManager.GetFeatureClass(BikeName);
            FlowFeatureClass = SDEManager.GetFeatureClass(FlowName);
            if (RoadFeatureClass == null||BusLineFeatureClass==null||BusStopFeatureClass==null||ParkingFeatureClass==null||BikeFeatureClass==null||FlowFeatureClass==null)
            {
                MessageBox.Show("未获取服务器上相关路网数据，请核对是否连接服务器.......");
            }*/
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
            switch (this.dataType)
            {
                case DataType.Road:
                    RoadFilterWhereClause = toolStripStatusLabel1.Text;
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter://
                            UpdateBase(RoadName, RoadFilterWhereClause);
                            break;
                        case InquiryMode.Search:
                            ShowResult(RoadFeatureClass,RoadFilterWhereClause);
                            break;
                    }
                    break;
                case DataType.Parking:
                    ParkingWhereClause = toolStripStatusLabel1.Text;
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            UpdateBase(ParkingName, ParkingWhereClause);
                            break;
                        case InquiryMode.Search:
                            ShowResult(ParkingFeatureClass,ParkingWhereClause);
                            break;
                    }
                    break;
                case DataType.Bike:
                    BikeWhereClause = toolStripStatusLabel1.Text;
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            UpdateBase(BikeName, BikeWhereClause);
                            break;
                        case InquiryMode.Search:
                            ShowResult(BikeFeatureClass, BikeWhereClause);
                            break;
                    }
                    break;
                case DataType.Flow:
                    FlowWhereClause = toolStripStatusLabel1.Text;
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            UpdateBase(FlowName, FlowWhereClause);
                            break;
                        case InquiryMode.Search:
                            ShowResult(FlowFeatureClass, FlowWhereClause);
                            break;
                    }
                    break;
                case DataType.BusLine:
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            break;
                        case InquiryMode.Search:
                            break;
                    }
                    break;
                case DataType.BusStop:
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            break;
                        case InquiryMode.Search:
                            break;
                    }
                    break;
            }
        }
        private void UpdateBase(string Name,string WhereClause)
        {
            var layers = LayerInfoHelper.GetLayers(Name);
            IFeatureLayer featureLayer = null;
            IFeatureLayerDefinition featureLayerDefinition = null;
            string LayerWhereClause = string.Empty;
            for (var i = 0; i < axMapControl1.Map.LayerCount; i++)
            {
                if (axMapControl1.Map.get_Layer(i) is GroupLayer)
                {
                    ICompositeLayer compositeLayer = axMapControl1.Map.get_Layer(i) as ICompositeLayer;
                    for (var j = 0; j < compositeLayer.Count; j++)
                    {
                        featureLayer = compositeLayer.get_Layer(j) as IFeatureLayer;
                        if (layers.Contains(featureLayer.Name))
                        {
                            featureLayerDefinition = featureLayer as IFeatureLayerDefinition;
                            if (Name == "AROAD")
                            {
                                if (string.IsNullOrEmpty(WhereClause))
                                {
                                    LayerWhereClause = "RANK ='" + featureLayer.Name + "'";
                                }
                                else
                                {
                                    LayerWhereClause = "RANK ='" + featureLayer.Name + "' AND " + WhereClause;
                                }
                            }
                            else
                            {
                                LayerWhereClause = WhereClause;
                            }
                            featureLayerDefinition.DefinitionExpression = LayerWhereClause;
                        }
                    }
                }
            }
            axMapControl1.ActiveView.Refresh();
        }
        /// <summary>
        /// 根据查询条件 更新路网数据
        /// </summary>
        public void UpdateRoad()
        {
            if (toolStripStatusLabel1.Text != RoadFilterWhereClause)
            {
                RoadFilterWhereClause = toolStripStatusLabel1.Text;
                UpdateBase(RoadName, RoadFilterWhereClause);
            }
        }
        public void UpdateBus()
        {
            switch (dataType)
            {
                case DataType.BusLine:
                    UpdateBase(BusLineName, BusLineWhereClause);
                    UpdateBase(BusStopName, BusStopWhereClause);
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

        #region  要素操作
        /// <summary>
        /// 要素闪烁
        /// </summary>
        /// <param name="Feature"></param>
        public void Twinkle(IFeature Feature)
        {
            if (Feature != null)
            {
                switch (Feature.Shape.GeometryType)
                {
                    case esriGeometryType.esriGeometryMultipoint:
                    case esriGeometryType.esriGeometryPoint:
                        axMapControl1.FlashShape(Feature.Shape, 4, 300, simpleMarkerSymbol);
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                    case esriGeometryType.esriGeometryLine:
                        axMapControl1.FlashShape(Feature.Shape, 4, 300, simpleLineSymbol);
                        break;
                    
                }
                
            }
        }
        /// <summary>
        /// 居中
        /// </summary>
        /// <param name="Feature"></param>
        public void Center(IFeature Feature)
        {
            if (Feature != null)
            {
                IEnvelope envelope = Feature.Shape.Envelope;
                IPoint point = new PointClass();
                point.PutCoords((envelope.XMin + envelope.XMax) / 2, (envelope.YMin + envelope.YMax) / 2);
                //axMapControl1.CenterAt(point);
                //居中方法二
                var env2 = axMapControl1.ActiveView.Extent;
                env2.CenterAt(point);
                axMapControl1.ActiveView.Extent = env2;
                axMapControl1.ActiveView.Refresh();
            }
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
        private void ExportSHP_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出路网SHP文件", "shp文件|*.shp");
            ExportSHPBase(RoadFeatureClass, RoadFilterWhereClause, saveFilePathSHP);
        }

        private void ExportBusLine_Click(object sender, EventArgs e)
        {
            var saveShpPath = FileHelper.Save("导出公交车路线Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BusLineFeatureClass, BusLineWhereClause, saveShpPath);
        }

        private void ExportBusStop_Click(object sender, EventArgs e)
        {
            var saveSHPPath = FileHelper.Save("导出公交车站点Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BusStopFeatureClass, BusStopWhereClause, saveSHPPath);
        }
        #endregion

        #region 导出图片
        private void ExportPictureBase(string FilePath, IActiveView ActiveView)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                FileHelper.ExportMap(FilePath, ActiveView);
                OperatorTxt.Text = "成功导出图片："+FilePath;
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
            ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
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
        private void ExportBusStopExcel_Click(object sender, EventArgs e)
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
        }

        #region  点选  路网 公交路线  公交站点  停车场

       
        private void ShowAttribute(IGeometry geometry)
        {
            IFeatureClass CurrentFeatureClass = null;
            string LayerName = string.Empty;
            switch (dataType)
            {
                case DataType.Road:
                    LayerName = RoadName;
                    CurrentFeatureClass = RoadFeatureClass;
                    break;
                case DataType.BusLine:
                    LayerName = BusLineName;
                    CurrentFeatureClass = BusLineFeatureClass;
                    break;
                case DataType.BusStop:
                    LayerName = BusStopName;
                    CurrentFeatureClass = BusStopFeatureClass;
                    break;
                case DataType.Parking:
                    LayerName = ParkingName;
                    CurrentFeatureClass = ParkingFeatureClass;
                    break;
                case DataType.Bike:
                    LayerName = BikeName;
                    CurrentFeatureClass = BikeFeatureClass;
                    break;
                case DataType.Flow:
                    LayerName = FlowName;
                    CurrentFeatureClass = FlowFeatureClass;
                    break;
            }
            IArray array = AttributeHelper.Identify(CurrentFeatureClass, geometry);
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

        /// <summary>
        /// 点击 点选查询路网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointSearch_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Road;
        }
        /// <summary>
        /// 公交车路线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointBusLineButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.BusLine;
        }
        /// <summary>
        /// 公交车站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointBusStopButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.BusStop;
        }
        /// <summary>
        /// 停车场
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointParkingButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Parking;
        }
        private void PointBikeButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Bike;
        }

        private void PointFlowButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Flow;
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
        /// <summary>
        /// 公交车路线搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBusLineButton_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.BusLine;
            BusFilterForm busform = new BusFilterForm();
            busform.ShowDialog(this);
        }
        /// <summary>
        /// 公交车站点搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBusStopButton_Click(object sender, EventArgs e)
        {
            this.dataType = DataType.BusStop;
            BusFilterForm busform = new BusFilterForm();
            busform.ShowDialog(this);
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
        private void ParkingFilter_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Parking,InquiryMode.Filter);   
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
            StatisticsForm statisticform = new StatisticsForm(ParkingFeatureClass,ParkingWhereClause,"ZHENGQU","BERTHNUM");
            statisticform.ShowDialog();
        }

        #endregion

        #region  交通流量监视器操作
        private void Operate(IGeometry geometry)
        {
            IFeatureClass currentFeatureClass = null;
            switch (this.dataType)
            {
                case DataType.Flow:
                    currentFeatureClass = FlowFeatureClass;
                    break;
                case DataType.Parking:
                    currentFeatureClass = ParkingFeatureClass;
                    break;
            }
            switch (this.operateMode)
            {
                case OperateMode.Add:
                    OperateForm operateform = new OperateForm(currentFeatureClass,geometry);
                    operateform.ShowDialog(this);
                    break;
                case OperateMode.Delete:
                     IArray array = AttributeHelper.Identify(currentFeatureClass, geometry);
                     if (array != null)
                     {
                         IFeatureIdentifyObj featureIdentifyObj = array.get_Element(0) as IFeatureIdentifyObj;
                         IIdentifyObj identifyObj = featureIdentifyObj as IIdentifyObj;
                         IRowIdentifyObject rowidentifyObject = featureIdentifyObj as IRowIdentifyObject;
                         IFeature feature = rowidentifyObject.Row as IFeature;
                         if (feature != null)
                         {
                             Twinkle(feature);
                             if (MessageBox.Show("您确定要删除当前选择交通流量检查器", "警告",MessageBoxButtons.OKCancel) == DialogResult.OK)
                             {
                                 feature.Delete();
                                 MapRefresh();
                             }
                         }
                     }
                    break;
                case OperateMode.Edit:
                     IArray arrayEdit = AttributeHelper.Identify(currentFeatureClass, geometry);
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

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            lblCoords.Text = string.Format("{0:#.#####},{1:#.#####}", e.mapX, e.mapY);
        }

        
    }
}
