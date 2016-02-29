using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Traffic.TForms
{
    public partial class ImportRoadForm : Form
    {
        private IFeatureClass m_RoadFeatureClass;
        private IFeatureClass m_RoadHistoryFC;
        private IFeatureClass m_RoadCrossFC;
        private readonly List<RoadCrossInfo> m_Crossroads = new List<RoadCrossInfo>();
        private List<IPolyline> m_Polylines;
        

        private bool canClose = false;
        private MainForm Father
        {
            get
            {
                return (MainForm)this.Owner;
            }
        }

        public ImportRoadForm()
        {
            InitializeComponent();
        }

        public ImportRoadForm(List<IPolyline> newRoads, IFeatureClass fc, IFeatureClass historyFC, IFeatureClass crossFC, IList<RoadCrossInfo> infos)
        {
            InitializeComponent();
            m_RoadFeatureClass = fc;
            m_RoadHistoryFC = historyFC;
            m_RoadCrossFC = crossFC;
            m_Polylines = newRoads;
            m_Crossroads.Clear();
            m_Crossroads.AddRange(infos);
        }

        private void ImportRoadForm_Load(object sender, EventArgs e)
        {
            LoadCrossroad();
            panel1.Dock = DockStyle.Fill;
            panel2.Dock = DockStyle.Fill;
            panel2.Dock = DockStyle.Fill;
            panel3.Dock = DockStyle.Fill;
            UpdateStepUI();
        }

        private void LoadCrossroad()
        {
            listCrossroads.Items.Clear();

            var count = 1;
            foreach(var line in m_Crossroads)
            {
                foreach (var cross in line.Crossings)
                {
                    var item = new ListViewItem(new[] { count.ToString(), cross.Crossing.X.ToString(), cross.Crossing.Y.ToString(), cross.Road.No.ToString(), cross.Road.Text });
                    item.Tag = cross;
                    item.Checked = true;
                    listCrossroads.Items.Add(item);
                    count++;
                }
            }
        }

        private void LoadFragment()
        {
            var list = new List<CrossroadInfo>();
            for(var i=0;i<listCrossroads.Items.Count;i++)
            {
                var item = listCrossroads.Items[i];
                var cross = item.Tag as CrossingInfo;
                cross.Enabled = item.Checked;
            }
            RoadMerger.QueryFragments(m_Crossroads);
            
            lstFragments.Items.Clear();
            var count = 1;
            foreach(var line in m_Crossroads)
            {
                if (line.HeadCrossing != null)
                {
                    var item = new ListViewItem(new[] { count.ToString(), Math.Round(line.HeadLength,2).ToString() });
                    item.Tag = "0";
                    item.Checked = true;
                    lstFragments.Items.Add(item);
                    count++;
                }

                if (line.TailCrossing != null)
                {
                    var item = new ListViewItem(new[] { count.ToString(), Math.Round(line.TailLength, 2).ToString() });
                    item.Tag = "0";
                    item.Checked = true;
                    lstFragments.Items.Add(item);
                    count++;
                }
            }
        }

        private void listCrossroads_MouseClick(object sender, MouseEventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;

            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossingInfo;
            var f = m_RoadFeatureClass.GetFeature(item.Road.Id);
            dataGridView1.DataSource = AttributeHelper.GetTable(m_RoadFeatureClass, f, "AROAD");
        }

        private void btnFlashCross_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossingInfo;
            Father.Twinkle(item.Crossing);
        }

        private void btnFlashRoad_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossingInfo;
            var f = m_RoadFeatureClass.GetFeature(item.Road.Id);
            Father.Twinkle(f);
        }

        private void btnMoveCross_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossingInfo;
            Father.Center(item.Crossing);
        }

        private void btnMoveRoad_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossingInfo;
            var f = m_RoadFeatureClass.GetFeature(item.Road.Id);
            Father.Center(f);
        }


        private void ImportNewRoadAndSplitRoads()
        {
            for(var i=0;i<lstFragments.Items.Count;i++)
            {
                var item = lstFragments.Items[i];
                if(item.Checked == false){
                    if(item.Tag != null && item.Tag.ToString() == "0")
                    {
                        m_Crossroads[i].Head = null ;
                    }
                    else if(item.Tag != null && item.Tag.ToString() == "1")
                    {
                        m_Crossroads[i].Tail = null;
                    }
                }
            }

            List<string> newIds;
            Dictionary<string, string> oldIds;

            RoadMerger.UpdateRoads(m_Crossroads, m_RoadFeatureClass, m_RoadHistoryFC, m_RoadCrossFC, out newIds, out oldIds);


            lstResult.Items.Clear();
            var count = 1;

            foreach(var no in newIds)
            {
                var item = new ListViewItem(new[] { 
                    count.ToString(), 
                    no.ToString(),
                    "[新导入道路]",
                    "[无]","[无]"
                });

                item.Tag = no;
                lstResult.Items.Add(item);
                count++;
            }

            foreach (var r in oldIds)
            {
                var name = string.Empty;
                foreach(var c in m_Crossroads)
                {
                    foreach(var d in c.Crossings)
                    {
                        if(d.Road.No == r.Value) 
                        {
                            name = d.Road.Text;
                            break;
                        }
                    }
                }

                var item = new ListViewItem(new[] { 
                    count.ToString(), 
                    r.Key,
                    name ,
                    r.Value,name
                    });

                item.Tag = r.Key;
                lstResult.Items.Add(item);
                count++;
            }

            
        }

        private void btnFlashFragment_Click(object sender, EventArgs e)
        {
            if (lstFragments.SelectedItems.Count == 0) return;

            var item = lstFragments.SelectedItems[0];
            if (item.Tag != null && item.Tag.ToString() == "0")
            {
                Father.Twinkle(m_Crossroads[lstFragments.SelectedIndices[0]].Head);
            }
            else
            {
                Father.Twinkle(m_Crossroads[lstFragments.SelectedIndices[0]].Tail);
            }
        }

        private void btnMoveFragement_Click(object sender, EventArgs e)
        {
            if (lstFragments.SelectedItems.Count == 0) return;

            var item = lstFragments.SelectedItems[0];
            if (item.Tag != null && item.Tag.ToString() == "0")
            {
                Father.Center(m_Crossroads[lstFragments.SelectedIndices[0]].Head);
            }
            else
            {
                Father.Center(m_Crossroads[lstFragments.SelectedIndices[0]].Tail);
            }
        }

        
        private void btnFlashFinal_Click(object sender, EventArgs e)
        {
            if (lstResult.SelectedItems.Count == 0) return;

            var item = lstResult.SelectedItems[0];
            var cursor = m_RoadFeatureClass.Search(new QueryFilterClass { WhereClause = "NO_=" + item.Tag.ToString() }, true);
            var f = cursor.NextFeature();
            if(f != null)
            {
                Father.Twinkle(f);
            }
            Marshal.ReleaseComObject(cursor);
        }

        private void btnMoveFinal_Click(object sender, EventArgs e)
        {
            if (lstResult.SelectedItems.Count == 0) return;

            var item = lstResult.SelectedItems[0];
            var cursor = m_RoadFeatureClass.Search(new QueryFilterClass { WhereClause = "NO_=" + item.Tag.ToString() }, true);
            var f = cursor.NextFeature();
            if (f != null)
            {
                Father.Center(f);
            }
            Marshal.ReleaseComObject(cursor);
        }

        private void ImportRoadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (canClose) 
            {
                Father.EraseImportRoadCustomDrawing();
                return;
            }

            if(MessageBox.Show("关闭窗口将会丢失您输入的信息，是否确认退出？", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                Father.EraseImportRoadCustomDrawing();
            }
        }

        private void btnExit1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int m_currentStep = 0;
        

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (m_currentStep >= 0) m_currentStep--;
            UpdateStepUI();
        }

        private void btnNext1_Click(object sender, EventArgs e)
        {
            if(m_currentStep == 1)
            {
                if (MessageBox.Show("是否确认开始新增道路到数据库，并打断已有的道路？", "请确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel) return;
            }

            if (m_currentStep < 3) m_currentStep++;
            switch(m_currentStep)
            {
                case 1:
                    LoadFragment();
                    break;
                case 2:
                    try
                    {
                        ImportNewRoadAndSplitRoads();
                        canClose = true;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("更新数据库时发生错误:" + ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        m_currentStep = 2;
                    }
                    
                    break;
            }

            UpdateStepUI();
        }
        
        private void UpdateStepUI()
        {
            switch(m_currentStep)
            {
                case 0:
                    ribbonTab1.Text = "步骤1：选择交点";
                    ribbonPanel1.Visible = true;
                    ribbonPanel3.Visible = false;
                    ribbonPanel4.Visible = false;
                    btnNext.Enabled = true;
                    btnNext.Text = "下一步";
                    btnPrevious.Enabled = false;
                    panel1.BringToFront();
                    break;
                case 1:
                    ribbonTab1.Text = "步骤2：碎片修剪";
                    ribbonPanel1.Visible = false;
                    ribbonPanel3.Visible = true;
                    ribbonPanel4.Visible = false;
                    btnNext.Text = "开始导入";
                    btnNext.Enabled = true;
                    btnPrevious.Enabled = true;
                    panel2.BringToFront();
                    break;
                case 2:
                    ribbonTab1.Text = "步骤3：导入完成";
                    ribbonPanel1.Visible = false;
                    ribbonPanel3.Visible = false;
                    ribbonPanel4.Visible = true;
                    btnNext.Enabled = false;
                    btnPrevious.Enabled = false;
                    panel3.BringToFront();
                    break;
            }
        }
        
    }
}
