using ESRI.ArcGIS.Geodatabase;
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
    public partial class ChooseForm : Form
    {
        private MainForm Father { get; set; }
        private int Number { get; set; }
        private int Start { get; set; }
        private int End { get; set; }
        private List<FeatureResult> ResultList { get; set; }
        public ChooseForm()
        {
            InitializeComponent();
        }
        public ChooseForm(List<FeatureResult> List ,MainForm father)
        {
            InitializeComponent();
            this.Father = father;
            this.ResultList = List;
            Init();
        }
        private void Init()
        {
            if (this.Father.BusLineFeatureClass != null)
            {
                this.Number = this.Father.BusLineFeatureClass.Fields.FindField(System.Configuration.ConfigurationManager.AppSettings["BUSKEY"]);
                this.Start = this.Father.BusLineFeatureClass.FindField(System.Configuration.ConfigurationManager.AppSettings["START"]);
                this.End = this.Father.BusLineFeatureClass.FindField(System.Configuration.ConfigurationManager.AppSettings["END"]);
            }
        }
        private string GetValue(IFeature Feature, int Index)
        {
            string values = string.Empty;
            try
            {
                values = Feature.get_Value(Index).ToString();
            }
            catch
            {

            }
            return values;
        }

        private void ChooseForm_Load(object sender, EventArgs e)
        {
            if (this.ResultList != null)
            {
                this.Text += "   当前公交车路线查询到" + this.ResultList.Count + "条记录";
                if (this.ResultList.Count == 1)
                {
                    Number1.Text = GetValue(this.ResultList[0].Feature, Number);
                    Start1.Text = GetValue(this.ResultList[0].Feature, Start);
                    End1.Text = GetValue(this.ResultList[0].Feature, End);
                    this.splitContainer1.Panel2.Width = 0;
                }
                else if (this.ResultList.Count == 2)
                {
                    Number1.Text = GetValue(this.ResultList[0].Feature, Number);
                    Start1.Text = GetValue(this.ResultList[0].Feature, Start);
                    End1.Text = GetValue(this.ResultList[0].Feature, End);
                    Number2.Text = GetValue(this.ResultList[1].Feature, Number);
                    Start2.Text = GetValue(this.ResultList[1].Feature, Start);
                    End2.Text = GetValue(this.ResultList[1].Feature, End);
                }
            }
            else
            {
                MessageBox.Show("未找到相关记录");
                this.Close();
            }
        }

        private void OneFeature_Click(object sender, EventArgs e)
        {
            if (this.ResultList != null)
            {
                if (this.ResultList.Count >0)
                {
                    AttributeForm form = new AttributeForm(this.ResultList[0].Feature, Father.BusLineFeatureClass,Father.BusLineName);
                    form.ShowDialog(Father);
                }
            }
        }

        private void TwoFeature_Click(object sender, EventArgs e)
        {
            //if (this.Features != null)
            //{
            //    if (this.Features.Count == 2)
            //    {
            //        AttributeForm form = new AttributeForm(this.Features[1], this.FeatureClass, Father.BusLineName);
            //        form.ShowDialog(Father);
            //    }
            //}
        }

    }
}
