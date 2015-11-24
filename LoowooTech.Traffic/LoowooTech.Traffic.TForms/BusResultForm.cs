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
        private Form1 Father { get; set; }
        private Dictionary<int, IFeature> FeatureDict { get; set; }
        public BusResultForm(IFeatureClass FeatureClass,IFeatureClass BusStopFeatureClass,string WhereClause)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.WhereClause = WhereClause;
            this.BusStopFeatureClass=BusStopFeatureClass;
            
        }
        public BusResultForm()
        {
            InitializeComponent();
        }

        private void BusResultForm_Load(object sender, EventArgs e)
        {
            Father = (Form1)this.Owner;
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

        private void ViewButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (FeatureDict.ContainsKey(selectIndex))
                {
                    //IFeature feature = FeatureDict[selectIndex];

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
    }
}
