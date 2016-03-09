using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoowooTech.Traffic.Common;

namespace LoowooTech.Traffic.TForms
{
    public partial class Statistic2Form : Form
    {
        private Dictionary<string,double> _columnData1 { get; set; }
        private Dictionary<string,double> _columnData2 { get; set; }
        private string[] _xValue { get; set; }
        private double[] _yValue { get; set; }
        private string _tableName1 { get; set; }
        private string _tableName2 { get; set; }
        public Statistic2Form(Dictionary<string,double> dict1,Dictionary<string,double> dict2,string tableName1,string tableName2)
        {
            InitializeComponent();
            _columnData1 = dict1;
            _columnData2 = dict2;
            _tableName1 = tableName1;
            _tableName2 = tableName2;
        }

        private void DataBind(Dictionary<string,double> Dict)
        {
            if (Dict != null && Dict.Count > 0)
            {
                var count = Dict.Count;
                _xValue = new string[count];
                _yValue = new double[count];
                int serial = 0;
                foreach (var val in Dict)
                {
                    _yValue[serial] = Math.Round(val.Value, 1);
                    _xValue[serial] = val.Key;
                    serial++;
                }
            }
        }

        private void Statistic2Form_Load(object sender, EventArgs e)
        {
            DataBind(_columnData1);
            chart1.Series["总泊位数"].Color = System.Configuration.ConfigurationManager.AppSettings["STATISTICCOLOR1"].GetColor();
            chart1.Series["总泊位数"].Points.DataBindXY(_xValue, _yValue);
            chart1.Titles.Add(_tableName1);
            DataBind(_columnData2);
            chart2.Series["个数"].Color = System.Configuration.ConfigurationManager.AppSettings["STATISTICCOLOR2"].GetColor();
            chart2.Series["个数"].Points.DataBindXY(_xValue, _yValue);
            chart2.Titles.Add(_tableName2);
        }
    }
}
