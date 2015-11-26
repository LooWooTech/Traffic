using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Common;
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
    //public delegate void EventOperator(IGeometry geometry);
    public partial class MainForm : Form
    {
        #region 初始化
        private string MXDPath { get; set; }
        private string RoadName { get; set; }
        private string BusLineName { get; set; }
        private string BusStopName { get; set; }
        private string ParkingName { get; set; }
        private string BikeName { get; set; }
        private string FlowName { get; set; }
        public string RoadFilterWhereClause { get; set; }
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
        public InquiryMode inquiryMode { get; set; }
        public DataType dataType { get; set; }
        public OperateMode operateMode { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        private SimpleMarkerSymbolClass simpleMarkerSymbol { get; set; }
        private User CurrentUser { get; set; }
        public MainForm()
        {
            InitializeComponent();
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
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            RoadFeatureClass = SDEManager.GetFeatureClass(RoadName);
            BusLineFeatureClass = SDEManager.GetFeatureClass(BusLineName);
            BusStopFeatureClass = SDEManager.GetFeatureClass(BusStopName);
            ParkingFeatureClass = SDEManager.GetFeatureClass(ParkingName);
            BikeFeatureClass = SDEManager.GetFeatureClass(BikeName);
            FlowFeatureClass = SDEManager.GetFeatureClass(FlowName);
            if (RoadFeatureClass == null || BusLineFeatureClass == null || BusStopFeatureClass == null || ParkingFeatureClass == null || BikeFeatureClass == null || FlowFeatureClass == null)
            {
                MessageBox.Show("未获取服务器上相关路网数据，请核对是否连接服务器.......");
            }
        }
        #endregion

        #region  基础函数

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
                            ShowResult(RoadFeatureClass, RoadFilterWhereClause);
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
                            ShowResult(ParkingFeatureClass, ParkingWhereClause);
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
                    BusLineWhereClause = toolStripStatusLabel1.Text;
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            UpdateBase(BusLineName, BusLineWhereClause);
                            break;
                        case InquiryMode.Search:
                            break;
                    }
                    break;
                case DataType.BusStop:
                    BusStopWhereClause = toolStripStatusLabel1.Text;
                    switch (this.inquiryMode)
                    {
                        case InquiryMode.Filter:
                            UpdateBase(BusStopName, BusStopWhereClause);
                            break;
                        case InquiryMode.Search:
                            break;
                    }
                    break;
            }
        }

        private void UpdateBase(string Name, string WhereClause)
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
        public void ShowResult(IFeatureClass FeatureClass, string WhereClause)
        {
            AttributeForm2 form2 = new AttributeForm2(FeatureClass, WhereClause);
            form2.Show(this);
        }
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
        private void ExportPictureBase(string FilePath, IActiveView ActiveView)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                FileHelper.ExportMap(FilePath, ActiveView);
                OperatorTxt.Text = "成功导出图片：" + FilePath;
            }
        }
        private void ExportExcelBase(IFeatureClass FeatureClass, string WhereClause, string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                var HeadDict = GISHelper.GetFieldIndexDict(FeatureClass);
                ExcelHelper.SaveExcel(FeatureClass, WhereClause, FilePath, HeadDict);
                OperatorTxt.Text = "成功导出Excel文件：" + FilePath;
            }
        }
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

        #region  路网

        /// <summary>
        /// 路网条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton6_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Road, InquiryMode.Search);
        }

        /// <summary>
        /// 路网条件过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton8_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Road, InquiryMode.Filter);
        }

        /// <summary>
        /// 路网点击查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton7_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Road;
        }

        /// <summary>
        /// 路网导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton9_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存路网图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
        }

        /// <summary>
        /// 路网导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton11_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出路网属性表格", "2003 xls 文件|*.xls|2007 xlsx|*.xlsx");
            ExportExcelBase(RoadFeatureClass, RoadFilterWhereClause, saveFilePath);
        }
        /// <summary>
        /// 路网导出图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton10_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出路网SHP文件", "shp文件|*.shp");
            ExportSHPBase(RoadFeatureClass, RoadFilterWhereClause, saveFilePathSHP);
        }

        #endregion

        #region  公交

        /// <summary>
        /// 公交线路条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton13_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusLine, InquiryMode.Search);
        }

        /// <summary>
        /// 公交站点 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton14_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.BusStop, InquiryMode.Search);
        }

        /// <summary>
        /// 路线点选查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton15_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.BusLine;
        }

        /// <summary>
        /// 站点点选查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton16_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.BusStop;
        }

        /// <summary>
        /// 导出公交图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton17_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存公交图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
        }

        /// <summary>
        /// 导出公交路线图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton18_Click(object sender, EventArgs e)
        {
            var saveShpPath = FileHelper.Save("导出公交车路线Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BusLineFeatureClass, BusLineWhereClause, saveShpPath);
        }

        /// <summary>
        /// 导出公交站点 图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton19_Click(object sender, EventArgs e)
        {
            var saveSHPPath = FileHelper.Save("导出公交车站点Shapefile文件", "SHP文件|*.shp");
            ExportSHPBase(BusStopFeatureClass, BusStopWhereClause, saveSHPPath);
        }

        /// <summary>
        /// 导出公交路线Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton20_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出公交车路线Excel文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(BusLineFeatureClass, BusLineWhereClause, saveFilePath);
        }
        /// <summary>
        /// 导出公交站点Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton21_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出公交车站点Excel文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(BusStopFeatureClass, BusStopWhereClause, saveFilePath);
        }

        #endregion

        #region 停车设施

        /// <summary>
        /// 停车设施 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton28_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Parking, InquiryMode.Search);
        }

        /// <summary>
        /// 停车设施  点选查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton29_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Parking;
        }

        /// <summary>
        /// 停车设施 条件过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton30_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Parking, InquiryMode.Filter); 
        }

        /// <summary>
        /// 停车设施  导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton25_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存停车场图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
        }

        /// <summary>
        /// 停车设施 导出图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton26_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出停车设施SHP文件", "shp文件|*.shp");
            ExportSHPBase(ParkingFeatureClass, ParkingWhereClause, saveFilePathSHP);
        }

        /// <summary>
        /// 停车设施 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonButton27_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出停车场属性文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(ParkingFeatureClass, ParkingWhereClause, saveFilePath);
        }

        /// <summary>
        /// 停车设施  添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddParking_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Add;
            this.dataType = DataType.Parking;
        }

        /// <summary>
        /// 停车设施  编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditParking_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Parking;
        }

        /// <summary>
        /// 停车设施 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteParking_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Parking;
        }

        #endregion

        #region 交通流量监测器
        /// <summary>
        /// 交通流量 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowSearch_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Flow, InquiryMode.Search);
        }

        /// <summary>
        /// 交通流量  点选查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointFlowSearch_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Flow;
        }

        /// <summary>
        /// 交通流量 过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowFilter_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Flow, InquiryMode.Filter);
        }

        /// <summary>
        /// 交通流量  导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportFlowPicture_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存交通流量图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
        }

        /// <summary>
        /// 导出交通流量  图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportFlowSHP_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出交通流量SHP文件", "shp文件|*.shp");
            ExportSHPBase(FlowFeatureClass, FlowWhereClause, saveFilePathSHP);
        }

        /// <summary>
        /// 导出交通流量监测器 Excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportFlowExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出交通流量属性文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(FlowFeatureClass, FlowWhereClause, saveFilePath);
        }

        /// <summary>
        /// 添加交通流量监测器 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFlow_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Add;
            this.dataType = DataType.Flow;
        }

        /// <summary>
        /// 编辑交通流量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditFlow_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Edit;
            this.dataType = DataType.Flow;
        }
        /// <summary>
        /// 删除交通流量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DeleteFlow_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.operateMode = OperateMode.Delete;
            this.dataType = DataType.Flow;
        }
        #endregion

        #region  公共自行车

        /// <summary>
        /// 公共自行车 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BikeSearch_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Bike, InquiryMode.Search);
        }

        /// <summary>
        /// 公共自行车  点选查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointBikeSearch_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            this.dataType = DataType.Bike;
        }

        /// <summary>
        /// 公共自行车  过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterBike_Click(object sender, EventArgs e)
        {
            FilterBase(DataType.Bike, InquiryMode.Filter);
        }
        /// <summary>
        /// 公共自行车  导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExportBikePicture_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存公共自行车图片", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            ExportPictureBase(saveFilePath, axMapControl1.ActiveView);
        }

        /// <summary>
        /// 导出公共自行车  图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportBikeSHP_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出公共自行车SHP文件", "shp文件|*.shp");
            ExportSHPBase(BikeFeatureClass, BikeWhereClause, saveFilePathSHP);
        }

        /// <summary>
        /// 导出公共自行车Excel 文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportBikeExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出公共自行车属性文件", "2003文件|*.xls|2007文件|*.xlsx");
            ExportExcelBase(BikeFeatureClass, BikeWhereClause, saveFilePath);
        }
        #endregion

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
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
                    OperateForm operateform = new OperateForm(currentFeatureClass, geometry);
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
                            if (MessageBox.Show("您确定要删除当前选择交通流量检查器", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
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
        public void MapRefresh()
        {
            this.axMapControl1.ActiveView.Refresh();
        }
    }
}
