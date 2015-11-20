using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Traffic.Common;
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
    public partial class AttributeForm : Form
    {
        private IFeature Feature { get; set; }
        private IFeatureClass FeatureClass { get; set; }
        private string LayerName { get; set; }
        private IActiveView ActiveView { get; set; }
        private Form1 Father { get; set; }
        public AttributeForm(IFeature feature,IFeatureClass featureClass,string LayerName,IActiveView activeView)
        {
            InitializeComponent();
            this.Feature = feature;
            this.FeatureClass = featureClass;
            this.LayerName = LayerName;
            this.ActiveView = activeView;
        }
        public AttributeForm()
        {
            InitializeComponent();
        }

        private void AttributeForm_Load(object sender, EventArgs e)
        {
            if (Feature != null && !string.IsNullOrEmpty(Name))
            {
                dataGridView1.DataSource = AttributeHelper.GetTable(FeatureClass, Feature, LayerName);
                Father = (Form1)this.Owner;
            }
        }

        /// <summary>
        /// 点击闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwinkleButton_Click(object sender, EventArgs e)
        {
            Father.Twinkle(Feature);
        }

        /// <summary>
        /// 点击移动居中显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveButton_Click(object sender, EventArgs e)
        {
            Father.Center(Feature);
        }
    }
}
