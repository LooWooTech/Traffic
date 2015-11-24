﻿using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.Manager;
using LoowooTech.Traffic.Models;
using LoowooTech.Traffic.TForms;
using System;
using System.Data;
using System.Windows.Forms;

namespace LoowooTech.Traffic.TForms
{
    public delegate void EventOperator(IGeometry geometry);
    public partial class Form1 : Form
    {
        private string MXDPath { get; set; }
        private string RoadName { get; set; }
        private string BusLineName { get; set; }
        private string BusStopName { get; set; }
        public  string RoadFilterWhereClause { get; set; }
        public string BusLineWhereClause { get; set; }
        public string BusStopWhereClause { get; set; }
        private IFeatureClass RoadFeatureClass { get; set; }
        private IFeatureClass BusLineFeatureClass { get; set; }
        private IFeatureClass BusStopFeatureClass { get; set; }
        private bool RoadFlag { get; set; }
        public  RoadMode roadMode { get; set; }
        public DataType dataType { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        public Form1()
        {
            InitializeComponent();
            MXDPath = System.Configuration.ConfigurationManager.AppSettings["MXD"];
            RoadName = System.Configuration.ConfigurationManager.AppSettings["ROAD"];
            BusLineName = System.Configuration.ConfigurationManager.AppSettings["BUSLINE"];
            BusStopName = System.Configuration.ConfigurationManager.AppSettings["BUSSTOP"];
            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 99);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            RoadFeatureClass = SDEManager.GetFeatureClass(RoadName);
            BusLineFeatureClass = SDEManager.GetFeatureClass(BusLineName);
            BusStopFeatureClass = SDEManager.GetFeatureClass(BusStopName);
            if (RoadFeatureClass == null)
            {
                MessageBox.Show("未获取服务器上相关路网数据，请核对是否连接服务器.......");
            }
        }
        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        { 
            if (axMapControl1.MousePointer==esriControlsMousePointer.esriPointerIdentify)
            {
                IPoint point = new PointClass();
                point.PutCoords(e.mapX, e.mapY);
                IGeometry geometry = point as IGeometry;
                this.Invoke(new EventOperator(ShowAttribute), new[] { geometry });
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
        /// 过滤路网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoadFilter_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            roadMode = RoadMode.Filter;
            var roadFilterForm = new RoadFilterForm(RoadFeatureClass);
            roadFilterForm.ShowDialog(this);
        }
        /// <summary>
        /// 导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportActiveView_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存地图", "jpeg文件|*.jpeg|bmp文件|*.bmp|png文件|*.png|gif文件|*.gif");
            FileHelper.ExportMap(saveFilePath, axMapControl1.ActiveView);
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
                            //Console.WriteLine(LayerWhereClause);
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
            UpdateBase(BusLineName, BusLineWhereClause);
            UpdateBase(BusStopName, BusStopWhereClause);
        }
        public void ShowResult()
        {
            AttributeForm2 form2 = new AttributeForm2(RoadFeatureClass, toolStripStatusLabel1.Text);
            form2.Show(this);
        }
        /// <summary>
        ///  条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionSearchButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            roadMode = RoadMode.Search;
            var roadFilterForm = new RoadFilterForm(RoadFeatureClass);
            roadFilterForm.ShowDialog(this);
            
        }
        /// <summary>
        /// 要素闪烁
        /// </summary>
        /// <param name="Feature"></param>
        public void Twinkle(IFeature Feature)
        {
            if (Feature != null)
            {
                axMapControl1.FlashShape(Feature.Shape, 4, 300, simpleLineSymbol);
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
        /// <summary>
        /// 点击 点选查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointSearch_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            dataType = DataType.Road;
        }
        private void ExportSHP_Click(object sender, EventArgs e)
        {
            var saveFilePathSHP = FileHelper.Save("导出SHP文件", "shp文件|*.shp");
            //GISHelper.Save(RoadFeatureClass, RoadFilterWhereClause, saveFilePathSHP);
            try
            {
                GISHelper.Save2(RoadFeatureClass, RoadFilterWhereClause, saveFilePathSHP);  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            
            toolStripStatusLabel1.Text = "成功导出Shapefile";
        }
        private void ExportExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("导出路网属性表格", "2003 xls 文件|*.xls|2007 xlsx|*.xlsx");
            var HeadDict = GISHelper.GetFieldIndexDict(RoadFeatureClass);
            ExcelHelper.SaveExcel(RoadFeatureClass, RoadFilterWhereClause, saveFilePath,HeadDict);
            toolStripStatusLabel1.Text = "成功导出Excel表格";
        }

        public void ShowBus()
        {
            //var result = new AttributeForm2(BusLineFeatureClass, BusLineWhereClause);
            //result.Show(this);
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

        private void SearchBusButton_Click(object sender, EventArgs e)
        {
            BusFilterForm busform = new BusFilterForm();
            busform.ShowDialog(this);
        }

        private void PointBusLineButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            dataType=DataType.BusLine;
        }

        private void PointBusStopButton_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            dataType = DataType.BusStop;
        }
    }
}
