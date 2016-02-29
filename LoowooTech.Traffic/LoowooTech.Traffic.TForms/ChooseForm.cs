﻿using ESRI.ArcGIS.Geodatabase;
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
        private string Get(IFeature Feature)
        {
            return string.Format("{0}→{1}", GetValue(Feature, Start), GetValue(Feature, End));
        }

        private void ChooseForm_Load(object sender, EventArgs e)
        {
            if (this.ResultList != null)
            {
                this.Text += string.Format("    {0} 查询到{1}条记录", GetValue(this.ResultList[0].Feature, Number), this.ResultList.Count);
                
                this.button1.Text = Get(this.ResultList[0].Feature);
                if (this.ResultList.Count == 1)
                {
                    this.button2.Visible = false;
                    this.label2.Visible = false;
                    this.Height = 180;
                }
                else
                {
                    this.button2.Text = Get(this.ResultList[1].Feature);
                }
            }
            else
            {
                MessageBox.Show("当前无记录");
                this.Close();
            }
        }

        private void MClick(string RoadWhereClause,string StopWhereClause)
        {
            Father.UpdateBase(Father.BusLineName, RoadWhereClause, Father.BusLineFeatureClass, true);
            Father.UpdateBase(Father.BusStopName, StopWhereClause, Father.BusStopFeatureClass);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MClick(this.ResultList[0].RoadWhereClause, this.ResultList[0].StopWhereClause);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MClick(this.ResultList[1].RoadWhereClause, this.ResultList[1].StopWhereClause);
        }
        private void View(IFeature Feature)
        {
            AttributeForm form = new AttributeForm(Feature, Father.BusLineFeatureClass, null, null, Father.BusLineName);
            form.Show(Father);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            View(this.ResultList[0].Feature);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            View(this.ResultList[1].Feature);
        }

    }
}
