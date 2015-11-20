using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Common;
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
        public  string RoadFilterWhereClause { get; set; }
        private IFeatureClass RoadFeatureClass { get; set; }
        private bool RoadFlag { get; set; }
        public  RoadMode roadMode { get; set; }
        private SimpleLineSymbolClass simpleLineSymbol { get; set; }
        public Form1()
        {
            InitializeComponent();
            MXDPath = System.Configuration.ConfigurationManager.AppSettings["MXD"];
            RoadName = System.Configuration.ConfigurationManager.AppSettings["ROAD"];
            simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = 4;
            simpleLineSymbol.Color = DisplayHelper.GetRGBColor(255, 0, 99);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RoadFeatureClass = SDEManager.GetFeatureClass(RoadName);
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
            IArray array = AttributeHelper.Identify(RoadFeatureClass, geometry);
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
                     //axMapControl1.FlashShape(feature.Shape, 5, 100, simpleLineSymbol);
                    //if (geometry.GeometryType == esriGeometryType.esriGeometryPoint)
                    //{
                    //    var point = geometry as IPoint;
                    //    MessageBox.Show(string.Format("X:{0} Y:{1}", point.X, point.Y));
                    //}
                    AttributeForm form = new AttributeForm(feature, RoadFeatureClass, RoadName,axMapControl1.ActiveView);
                    form.ShowDialog(this);
                }
            }
            
        }

        //private void AttributeButton_Click(object sender, EventArgs e)
        //{
        //    RoadFlag = !RoadFlag;
        //    AttributeButton.Checked = RoadFlag;
        //    AttributeButton.CheckState = RoadFlag ? CheckState.Checked : CheckState.Unchecked;
        //}


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

        /// <summary>
        /// 根据查询条件 更新路网数据
        /// </summary>
        public void UpdateRoad()
        {
            if (toolStripStatusLabel1.Text != RoadFilterWhereClause)
            {
                RoadFilterWhereClause = toolStripStatusLabel1.Text;
                var layers = LayerInfoHelper.GetLayers(RoadName);
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
                                if (string.IsNullOrEmpty(RoadFilterWhereClause))
                                {
                                    LayerWhereClause = "RANK ='" + featureLayer.Name + "'";
                                }
                                else
                                {
                                    LayerWhereClause = "RANK ='" + featureLayer.Name + "' AND " + RoadFilterWhereClause;
                                }
                                
                                Console.WriteLine(LayerWhereClause);
                                featureLayerDefinition.DefinitionExpression = LayerWhereClause;
                            }
                        }
                    }
                }
                axMapControl1.ActiveView.Refresh();
            }
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
        public void Center(IFeature Feature)
        {
            if (Feature != null)
            {
                IGeometry geometry = Feature.Shape as IGeometry;
               
                //axMapControl1.CenterAt((IPoint)Feature.Shape);
                //Application.DoEvents();
            }
        }
        public void Query()
        {
            if (toolStripStatusLabel1.Text != RoadFilterWhereClause)
            {
                RoadFilterWhereClause = toolStripStatusLabel1.Text;

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
        }
    }
}
