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
using System.Windows.Forms.DataVisualization.Charting;

namespace LoowooTech.Traffic.TForms
{
    public partial class StatisticsForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private string WhereClause { get; set; }
        private string LabelName { get; set; }
        private string FieldName { get; set; }
        private Dictionary<string, double> ColumneData { get; set; }
        public StatisticsForm(IFeatureClass FeatureClass,string WhereClause,string LabelName,string FieldName)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.WhereClause = WhereClause;
            this.LabelName = LabelName;
            this.FieldName = FieldName;
        }
        public StatisticsForm()
        {
            InitializeComponent();
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            var dict = GISHelper.Statistic(FeatureClass,LabelName,FieldName);
            var count = dict.Count;
            double[] yValues = new double[count];
            string[] xValues = new string[count];
            int Serial = 0;
            foreach (var key in dict.Keys)
            {
                yValues[Serial] = dict[key];
                xValues[Serial] = key;
                Serial++;
            }
            try
            {
                chart1.Series["Series1"].Points.DataBindXY(xValues, yValues);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
