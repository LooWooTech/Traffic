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
    public partial class BusResultForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private IFeatureClass BusStopFeatureClass { get; set; }
        private string WhereClause { get; set; }
        private MainForm Father { get; set; }
        private Dictionary<int, IFeature> FeatureDict { get; set; }
        private int IndexDirect { get; set; }
        private int IndexNameShort { get; set; }
        public BusResultForm(IFeatureClass FeatureClass,IFeatureClass BusStopFeatureClass,string WhereClause)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.WhereClause = WhereClause;
            this.BusStopFeatureClass=BusStopFeatureClass;
            this.IndexDirect = FeatureClass.Fields.FindField("lineDirect");
            this.IndexNameShort = FeatureClass.Fields.FindField("nameshort");
            
        }
        public BusResultForm()
        {
            InitializeComponent();
        }

        private void BusResultForm_Load(object sender, EventArgs e)
        {
            Father = (MainForm)this.Owner;
            if (FeatureClass != null)
            {
                Dictionary<int, IFeature> temp;
                dataGridView1.DataSource = AttributeHelper.GetTable(FeatureClass, WhereClause, out temp);
                if (temp != null) 
                {
                    FeatureDict = temp;
                }
            }
        }

        /// <summary>
        /// 双击  某一行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
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
        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwinkleButton_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 查看某一条公交路线的公交站点信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (FeatureDict.ContainsKey(selectIndex))
                {
                    IFeature Currentfeature = FeatureDict[selectIndex];
                    if (Currentfeature != null)
                    {
                        var direct = int.Parse(Currentfeature.get_Value(IndexDirect).ToString());
                        var nameshort = Currentfeature.get_Value(IndexNameShort).ToString();
                        BusStopForm form = new BusStopForm(BusStopFeatureClass, "lineDirect= "+direct+"  AND lineNameshort='"+nameshort+"'");
                        form.ShowDialog(this.Owner);
                    }
                    

                }
            }
        }
        /// <summary>
        /// 移动  居中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveButton_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 导出结果Excel表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportExcel_Click(object sender, EventArgs e)
        {
            var saveFilePath = FileHelper.Save("保存Excel表格", "2003 xls文件|*.xls|2007 xlsx|*.xlsx");
            var HeadDict = GISHelper.GetFieldIndexDict(FeatureClass, "序号");
            Father.toolStripStatusLabel1.Text = "正在生成文件：" + saveFilePath;
            ExcelHelper.SaveExcel(dataGridView1.DataSource as DataTable, saveFilePath, HeadDict);
            Father.toolStripStatusLabel1.Text = "文件生成：" + saveFilePath;
        }

        private void ribbonButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
