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
        private IFeatureClass HistoryFC { get; set; }
        private IFeatureClass CrossFC { get; set; }
        private string LayerName { get; set; }
        private MainForm Father { get; set; }
        public AttributeForm(IFeature feature, IFeatureClass featureClass, IFeatureClass historyFC, IFeatureClass crossFC, string LayerName)
        {
            InitializeComponent();
            this.Feature = feature;
            this.FeatureClass = featureClass;
            this.LayerName = LayerName;
            this.HistoryFC = historyFC;
            this.CrossFC = crossFC;
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
                dataGridView1.Tag = Feature.OID;
                Father = (MainForm)this.Owner;
                
            }
            if (LayerName != "路网")
            {
                this.btnHistory.Enabled = false;
            }
            else
            {
                this.btnHistory.Enabled = true;
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

        private void btnHistory_Click(object sender, EventArgs e)
        {
            var form = new HistoryForm((int)dataGridView1.Tag, FeatureClass, HistoryFC, CrossFC, Father);
            form.ShowDialog();
        }
    }
}
