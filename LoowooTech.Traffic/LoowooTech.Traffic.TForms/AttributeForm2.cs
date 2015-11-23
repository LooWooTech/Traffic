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
    public partial class AttributeForm2 : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private string WhereClause { get; set; }
        private Form1 Father { get; set; }
        private Dictionary<int, IFeature> FeatureDict { get; set; }
        public AttributeForm2(IFeatureClass featureClass, string WhereClause)
        {
            InitializeComponent();
            this.FeatureClass = featureClass;
            this.WhereClause = WhereClause;
        }
        public AttributeForm2()
        {
            InitializeComponent();
        }

        private void AttributeForm2_Load(object sender, EventArgs e)
        {
            Father = (Form1)this.Owner;
            if (FeatureClass != null)
            {
                Dictionary<int, IFeature> temp;
                dataGridView1.DataSource = AttributeHelper.GetTable(FeatureClass, WhereClause,out temp);
                if (temp != null)
                {
                    FeatureDict = temp;
                }
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
                    
                    Father.Twinkle(FeatureDict[selectIndex]);
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
            var HeadDict = GISHelper.GetFieldIndexDict(FeatureClass,"序号");
            Father.toolStripStatusLabel1.Text = "正在生成文件："+saveFilePath;
            ExcelHelper.SaveExcel(dataGridView1.DataSource as DataTable, saveFilePath,HeadDict);
            Father.toolStripStatusLabel1.Text = "文件生成：" + saveFilePath;
        }
    }
}
