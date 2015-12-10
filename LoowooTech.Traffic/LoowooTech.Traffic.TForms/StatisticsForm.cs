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
        private Dictionary<string, int> ColumnIntData { get; set; }


        public StatisticsForm(Dictionary<string, double> ColumnDict,string TableName)
        {
            InitializeComponent();
            this.ColumneData = ColumnDict;
            this.label1.Text = TableName;
            this.label2.Text = "";
        }
        public StatisticsForm(Dictionary<string, int> ColumnDict, string TableName,int Sum) 
        {
            InitializeComponent();
            this.ColumnIntData = ColumnDict;
            this.label1.Text = TableName;
            this.label2.Text = "当前区域总泊位数为：" + Sum;
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
            else if (ColumnIntData != null && ColumnIntData.Count > 0)
            {
                var count = ColumnIntData.Count;
                int[] yvalue = new int[count];
                string[] xvalue = new string[count];
                int Serial = 0;
                foreach (var key in ColumnIntData.Keys)
                {
                    yvalue[Serial] = ColumnIntData[key];
                    xvalue[Serial] = key;
                    Serial++;
                }
                chart1.Series["Series1"].Points.DataBindXY(xvalue, yvalue);
            }
           
        }
    }
}
