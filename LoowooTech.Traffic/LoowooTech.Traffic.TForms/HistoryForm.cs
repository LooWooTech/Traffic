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
    public partial class HistoryForm : Form
    {
        private IFeatureClass m_RoadFC;
        private IFeatureClass m_RoadNodeFC;
        private IFeatureClass m_RoadHistoryFC;
        private MainForm m_Parent;
        private int m_fid;

        public HistoryForm()
        {
            InitializeComponent();
            /*var item = new ListViewItem(new[] { "1", "3538" , "3773", "2015-09-01 12:05:01" });
            lstResult.Items.Add(item);

            var item2 = new ListViewItem(new[] { "1", "3225", "3538", "2014-8-10 08:12:45" });
            lstResult.Items.Add(item2);*/
        }

        public HistoryForm(int fid, IFeatureClass roadFC, IFeatureClass roadHistoryFC, IFeatureClass roadNodeFC, MainForm parent)
        {
            InitializeComponent();
            m_fid = fid;
            m_RoadFC = roadFC;
            m_RoadHistoryFC = roadHistoryFC;
            m_RoadNodeFC = roadNodeFC;
            m_Parent = parent;
        }

        private void LoadData()
        {
            lstResult.Items.Clear();

            var f = m_RoadFC.GetFeature(m_fid);
            var item = new ListViewItem(new[] { "1", 
                f.get_Value(f.Fields.FindField(RoadMerger.ParentIDFieldName)).ToString(), 
                f.get_Value(f.Fields.FindField(RoadMerger.IDFieldName)).ToString(),
                string.Format("{0:yyyy-MM-dd HH:mm:ss}", f.get_Value(f.Fields.FindField(RoadMerger.CreateTimeFieldName)))});
           
            lstResult.Items.Add(item);

            var list = RoadMerger.GetHistoryList(m_fid, m_RoadFC, m_RoadHistoryFC);
            var index = 2;
            foreach(var id in list )
            {
                f = m_RoadHistoryFC.GetFeature(id);
                item = new ListViewItem(new[] { index.ToString(), 
                f.get_Value(f.Fields.FindField(RoadMerger.ParentIDFieldName)).ToString(), 
                f.get_Value(f.Fields.FindField(RoadMerger.IDFieldName)).ToString(),
                string.Format("{0:yyyy-MM-dd HH:mm:ss}", f.get_Value(f.Fields.FindField(RoadMerger.CreateTimeFieldName)))});
                item.Tag = id;
                lstResult.Items.Add(item);
                index++;
            }
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFlash_Click(object sender, EventArgs e)
        {
            if(m_Parent != null)
            {
                if(lstResult.SelectedIndices.Count > 0)
                {
                    if(lstResult.SelectedIndices[0] == 0)
                    {
                        m_Parent.Twinkle(m_RoadFC.GetFeature(m_fid));
                    }
                    else
                    {
                        m_Parent.Twinkle(m_RoadHistoryFC.GetFeature((int)lstResult.Items[lstResult.SelectedIndices[0]].Tag));
                    }
                }
            }
        }

        private void btnZoom_Click(object sender, EventArgs e)
        {
            if (m_Parent != null)
            {
                if (lstResult.SelectedIndices.Count > 0)
                {
                    if (lstResult.SelectedIndices[0] == 0)
                    {
                        m_Parent.Twinkle(m_RoadFC.GetFeature(m_fid));
                    }
                    else
                    {
                        m_Parent.Twinkle(m_RoadHistoryFC.GetFeature((int)lstResult.Items[lstResult.SelectedIndices[0]].Tag));
                    }
                }
            }
        }

        private void btnRecover_Click(object sender, EventArgs e)
        {
            if (lstResult.SelectedIndices.Count > 0)
            {
                if (lstResult.SelectedIndices[0] == 0)
                {
                    MessageBox.Show("您当前选择的记录不是历史记录，请选择其他记录重试。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var f = m_RoadHistoryFC.GetFeature((int)lstResult.Items[lstResult.SelectedIndices[0]].Tag);
                    if (MessageBox.Show("退回到历史记录的操作将会删除道路现状数据，是否确认继续恢复？", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {

                        var ret = RoadMerger.HistoryRecover(Convert.ToInt32(f.get_Value(f.Fields.FindField(RoadMerger.IDFieldName))), m_RoadFC, m_RoadHistoryFC, m_RoadNodeFC);
                        if (ret)
                        {
                            MessageBox.Show("已成功完成退回到历史记录操作", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("退回到历史记录操作中出现错误，请联系管理员对历史记录库进行清理", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }
    }
}
