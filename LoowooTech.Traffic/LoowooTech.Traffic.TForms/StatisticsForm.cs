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
        private Dictionary<string, double> ColumneData { get; set; }

        public StatisticsForm(Dictionary<string, double> ColumnDict,string TableName)
        {
            InitializeComponent();
            this.ColumneData = ColumnDict;
        }
        public StatisticsForm()
        {
            InitializeComponent();
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            if (ColumneData != null&&ColumneData.Count>0)
            {
                var count = ColumneData.Count;
                double[] yValue = new double[count];
                string[] xValue = new string[count];
                int Serial = 0;
                foreach (var key in ColumneData.Keys)
                {
                    yValue[Serial] = ColumneData[key];
                    xValue[Serial] = key;
                    Serial++;
                }
                chart1.Series["Series1"].Points.DataBindXY(xValue, yValue);
            }           
        }
    }
}
