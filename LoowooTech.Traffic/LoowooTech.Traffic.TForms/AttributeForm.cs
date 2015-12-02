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
        private MainForm Father { get; set; }
        public AttributeForm(IFeature feature,IFeatureClass featureClass,string LayerName)
        {
            InitializeComponent();
            this.Feature = feature;
            this.FeatureClass = featureClass;
            this.LayerName = LayerName;
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
                Father = (MainForm)this.Owner;
            }
            if (LayerName != "AROAD")
            {
                this.RoadAndBus.Visible = false;
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

        private void ribbonButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RoadAndBus_Click(object sender, EventArgs e)
        {
            this.Invoke(new EventOperator(Father.Analyze2), new[] { this.Feature.Shape });
        }
    }
}
