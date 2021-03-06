﻿using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
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
    public partial class AttributeForm2 : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private IFeatureClass FeatureClass2 { get; set; }
        private IGeometry geometry { get; set; }
        private IFeature Feature { get; set; }
        private SpaceMode Mode { get; set; }
        private DataType dataType { get; set; }
        private string Title { get; set; }
        private string WhereClause { get; set; }
        private MainForm Father { get; set; }
        private Dictionary<int, IFeature> FeatureDict { get; set; }
        public AttributeForm2(IFeatureClass featureClass, string WhereClause)
        {
            InitializeComponent();
            this.FeatureClass = featureClass;
            this.WhereClause = WhereClause;
        }
        public AttributeForm2(IFeatureClass FeatureClass, IGeometry geometry, SpaceMode mode,string Title,DataType dataType,IFeatureClass FeatureClass2=null,IFeature Feature=null)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.geometry = geometry;
            this.Mode = mode;
            this.Title = Title;
            this.dataType = dataType;
            this.FeatureClass2 = FeatureClass2;
            this.Feature = Feature;
        }
        public AttributeForm2()
        {
            InitializeComponent();
        }

        private void AttributeForm2_Load(object sender, EventArgs e)
        {
            if (this.dataType != DataType.Parking)
            {
                this.BtnStatistic.Visible = false;
            }
            Father = (MainForm)this.Owner;
            if (FeatureClass != null)
            {
                Dictionary<int, IFeature> temp;
                dataGridView1.DataSource = AttributeHelper.GetTable(FeatureClass, WhereClause, out temp,geometry,Mode);
                if (temp != null)
                {
                    FeatureDict = temp;
                }
            }
            if (!string.IsNullOrEmpty(this.Title))
            {
                this.Text = this.Title;
            }
        }
        
        
        /// <summary>
        /// 双击 列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine("双击");
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (FeatureDict.ContainsKey(selectIndex))
                {
                    Father.Center(FeatureDict[selectIndex]);

                }
            }
            
        }

        private void Twinklebutton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (FeatureDict.ContainsKey(selectIndex))
                {
                    Father.Twinkle(FeatureDict[selectIndex]);
                }
            }
        }

        private void ExportExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存Excel表格", "2003 xls文件|*.xls|2007 xlsx|*.xlsx");
            if (!string.IsNullOrEmpty(saveFilePath))
            {
                //var HeadDict = GISHelper.GetFieldIndexDict(FeatureClass, "序号");
                Father.toolStripStatusLabel1.Text = "正在生成文件：" + saveFilePath;
                ExcelHelper.SaveExcel(dataGridView1.DataSource as DataTable, saveFilePath);
                Father.toolStripStatusLabel1.Text = "文件生成：" + saveFilePath;
            }
            
        }

        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (FeatureDict.ContainsKey(selectIndex))
                {
                    Father.Center(FeatureDict[selectIndex]);
                }
            }
        }

        private void ribbonButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnStatistic_Click(object sender, EventArgs e)
        {
            string Parkingkey1 = System.Configuration.ConfigurationManager.AppSettings["PARKINGKEY1"];
            string Parkingkey2 = System.Configuration.ConfigurationManager.AppSettings["PARKINGKEY2"];
            var dict = ExcelHelper.Statistic(dataGridView1.DataSource as DataTable, Parkingkey1);
            var dict2 = ExcelHelper.Statistic2(dataGridView1.DataSource as DataTable, Parkingkey1,Parkingkey2);
            //var sum = ExcelHelper.Statistic2(dataGridView1.DataSource as DataTable, Parkingkey2);
            var form= new Statistic2Form(dict2, dict, "当前框选区域停车场泊车位统计图", "当前框选区域停车场个数统计图");
            //StatisticsForm form = new StatisticsForm(dict, "当前区域内停车设施情况",sum,Father.ParkingName,Parkingkey1);
            form.ShowDialog();
        }

        private void ribbon1_TabIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(ribbon1.TabIndex);
        }

        private void ribbon1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(ribbon1.TabIndex);
        }       
    }
}
