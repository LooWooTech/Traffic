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
        private IFeatureClass m_DistrictClass;
        private readonly List<CrossroadInfo> m_Crossroads = new List<CrossroadInfo>();
        private IPolyline m_Polyline;
        private EditAttributeControl editAttributeControl1 = new EditAttributeControl();

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

        public ImportRoadForm(IPolyline newRoad, IFeatureClass fc, IFeatureClass districtFC, IList<CrossroadInfo> infos)
        {
            InitializeComponent();
            m_RoadFeatureClass = fc;
            m_DistrictClass = districtFC;
            m_Polyline = newRoad;
            m_Crossroads.Clear();
            m_Crossroads.AddRange(infos);
        }

        private void ImportRoadForm_Load(object sender, EventArgs e)
        {
            LoadCrossroad();
            panel1.Dock = DockStyle.Fill;
            panel2.Dock = DockStyle.Fill;
            panel3.Dock = DockStyle.Fill;
            panel4.Dock = DockStyle.Fill;
            tabPage4.Controls.Add(editAttributeControl1);
            editAttributeControl1.Dock = DockStyle.Fill;
            editAttributeControl1.Initialize(m_RoadFeatureClass);
            UpdateStepUI();
        }

        private void LoadCrossroad()
        {
            listCrossroads.Items.Clear();

            var count = 1;
            foreach(var line in m_Crossroads)
            {
                var item = new ListViewItem(new[] { count.ToString(), line.Point.X.ToString(), line.Point.Y.ToString(), line.NO.ToString(), line.Name });
                item.Tag = line;
                item.Checked = true;
                listCrossroads.Items.Add(item);
                count++;
            }
        }

        private IPolyline m_HeadPolyline;
        private IPolyline m_TailPolyline;

        
        private void LoadFragment()
        {
            var list = new List<CrossroadInfo>();
            for(var i=0;i<listCrossroads.Items.Count;i++)
            {
                var item = listCrossroads.Items[i];
                if (item.Checked)
                {
                    list.Add(item.Tag as CrossroadInfo);
                }
            }

            var polylines = RoadHelper.SplitPolylineInner(m_Polyline, list.Select(x => x.Point).ToList());
            lstFragments.Items.Clear();
            var count = 1;
            if(polylines.Count>0)
            {
                var length = GetProjectedLength( polylines[0]);
                if(length < 100)
                {
                    var item = new ListViewItem(new[] { count.ToString(), length.ToString()});
                    item.Tag = "0";
                    item.Checked = true;
                    m_HeadPolyline = polylines[0];
                    lstFragments.Items.Add(item);
                    count++;
                }

                length = GetProjectedLength(polylines[polylines.Count - 1]);
                if(polylines.Count>1 && length < 100)
                {
                    var item = new ListViewItem(new[] { count.ToString(), length.ToString() });
                    item.Tag = "1";
                    item.Checked = true;
                    m_TailPolyline = polylines[polylines.Count - 1];
                    lstFragments.Items.Add(item);
                    count++;
                }                
            }
        }

        private double GetProjectedLength(IPolyline line)
        {
            var copy = new ObjectCopyClass();
            var l2 = copy.Copy(line) as IPolyline;

            var factory = new SpatialReferenceEnvironmentClass();
            var fromSR = factory.CreateGeographicCoordinateSystem((int)esriSRGeoCS3Type.esriSRGeoCS_Xian1980);//西安80
            var toSR = factory.CreateProjectedCoordinateSystem((int)esriSRProjCS4Type.esriSRProjCS_Xian1980_3_Degree_GK_Zone_40);//西安80
            l2.SpatialReference = fromSR;
            l2.Project(toSR);
            return l2.Length;
        }

        private void listCrossroads_MouseClick(object sender, MouseEventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;

            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossroadInfo;
            var f = m_RoadFeatureClass.GetFeature(item.OID);
            dataGridView1.DataSource = AttributeHelper.GetTable(m_RoadFeatureClass, f, "AROAD");
        }

        private void btnFlashCross_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossroadInfo;
            Father.Twinkle(item.Point);
        }

        private void btnFlashRoad_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossroadInfo;
            var f = m_RoadFeatureClass.GetFeature(item.OID);
            Father.Twinkle(f);
        }

        private void ImportNewRoadAndSplitRoads()
        {
            var dropHead = false;
            var dropTail = false;

            for(var i=0;i<lstFragments.Items.Count;i++)
            {
                var item = lstFragments.Items[i];
                if(item.Checked){
                    if(item.Tag != null && item.Tag.ToString() == "0")
                    {
                        dropHead = true;
                    }
                    else if(item.Tag != null && item.Tag.ToString() == "1")
                    {
                        dropTail = true;
                    }
                }
            }
            var list = new List<CrossroadInfo>();
            for(var i=0;i<listCrossroads.Items.Count;i++)
            {
                var item = listCrossroads.Items[i];
                if (item.Checked)
                {
                    list.Add(item.Tag as CrossroadInfo);
                }
            }

            //var polylines = RoadHelper.SplitPolylineInner(m_Polyline, list.Select(x => x.Point).ToList());
            var newAttributes = editAttributeControl1.GetAttributes();
            var ret = RoadHelper.SplitPolyline(m_Polyline, list.Select(x => x.Point).ToList(), newAttributes , m_RoadFeatureClass,m_DistrictClass, dropHead, dropTail);
            
            lstResult.Items.Clear();
            var count = 1;
            foreach(var no in ret)
            {
                var item = new ListViewItem(new[] { 
                    count.ToString(), 
                    no.ToString(),
                    newAttributes["NAME"],
                    "[无]","[无]"
                });

                item.Tag = no;
                lstResult.Items.Add(item);
                count++;
            }

            var dict = new Dictionary<int, List<CrossroadInfo>>();
            
            foreach(var c in list)
            {
                if (dict.ContainsKey(c.OID) == false) dict.Add(c.OID, new List<CrossroadInfo>());

                dict[c.OID].Add(c);                
            }

            foreach(var pair in dict)
            {                
                var no = pair.Value[0].NO;
                var name = pair.Value[0].Name;
                var results = RoadHelper.SplitPolyline(pair.Key, pair.Value.Select(x=>x.Point).ToList(), m_RoadFeatureClass);
                foreach (var r in results)
                {
                    var item = new ListViewItem(new[] { 
                    count.ToString(), 
                    r.ToString(),
                    name,
                    no.ToString(),name
                    });

                    item.Tag = r;
                    lstResult.Items.Add(item);
                    count++;
                }
            }
        }

        private void btnFlashFragment_Click(object sender, EventArgs e)
        {
            if (lstFragments.SelectedItems.Count == 0) return;

            var item = lstFragments.SelectedItems[0];
            if (item.Tag != null && item.Tag.ToString() == "0")
            {
                Father.Twinkle(m_HeadPolyline);
            }
            else
            {
                Father.Twinkle(m_TailPolyline);
            }
        }

        private void btnMoveFragement_Click(object sender, EventArgs e)
        {
            if (lstFragments.SelectedItems.Count == 0) return;

            var item = lstFragments.SelectedItems[0];
            if (item.Tag != null && item.Tag.ToString() == "0")
            {
                Father.Center(m_HeadPolyline);
            }
            else
            {
                Father.Center(m_TailPolyline);
            }
        }

        private void btnMoveCross_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossroadInfo;
            Father.Center(item.Point);
        }

        private void btnMoveRoad_Click(object sender, EventArgs e)
        {
            if (listCrossroads.SelectedItems.Count == 0) return;
            var item = listCrossroads.Items[listCrossroads.SelectedIndices[0]].Tag as CrossroadInfo;
            var f = m_RoadFeatureClass.GetFeature(item.OID);
            Father.Center(f);
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
                Father.EraseImportRoadCustomDrawing()
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
            if(m_currentStep == 2)
            {
                if (MessageBox.Show("是否确认开始新增道路到数据库，并打断已有的道路？", "请确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel) return;
            }

            if (m_currentStep < 4) m_currentStep++;
            switch(m_currentStep)
            {
                case 1:
                    LoadFragment();
                    break;
                case 3:
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
                    ribbonTab1.Text = "步骤2：属性录入";
                    ribbonPanel1.Visible = false;
                    ribbonPanel3.Visible = false;
                    ribbonPanel4.Visible = false;
                    btnNext.Text = "下一步";
                    btnNext.Enabled = true;
                    btnPrevious.Enabled = true;
                    panel2.BringToFront();
                    break;
                case 2:
                    ribbonTab1.Text = "步骤3：碎片修剪";
                    ribbonPanel1.Visible = false;
                    ribbonPanel3.Visible = true;
                    ribbonPanel4.Visible = false;
                    btnNext.Text = "开始导入";
                    btnNext.Enabled = true;
                    btnPrevious.Enabled = true;
                    panel3.BringToFront();
                    break;
                case 3:
                    ribbonTab1.Text = "步骤4：导入完成";
                    ribbonPanel1.Visible = false;
                    ribbonPanel3.Visible = false;
                    ribbonPanel4.Visible = true;
                    btnNext.Enabled = false;
                    btnPrevious.Enabled = false;
                    panel4.BringToFront();
                    break;
            }
        }
        
    }
}
