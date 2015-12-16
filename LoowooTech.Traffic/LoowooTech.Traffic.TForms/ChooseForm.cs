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
    public partial class ChooseForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private string WhereClause { get; set; }
        private MainForm Father { get; set; }
        private int Number { get; set; }
        private int Start { get; set; }
        private int End { get; set; }
        private List<IFeature> Features { get; set; }
        public ChooseForm()
        {
            InitializeComponent();
        }
        public ChooseForm(IFeatureClass FeatureClass, string WhereClause,MainForm father)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.WhereClause = WhereClause;
            this.Father = father;
            Init();
        }
        private void Init()
        {
            if (this.FeatureClass != null)
            {
                this.Number = FeatureClass.Fields.FindField(System.Configuration.ConfigurationManager.AppSettings["BUSKEY"]);
                this.Start = FeatureClass.FindField(System.Configuration.ConfigurationManager.AppSettings["START"]);
                this.End = FeatureClass.FindField(System.Configuration.ConfigurationManager.AppSettings["END"]);
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
            this.Features = GISHelper.Search(FeatureClass, WhereClause);
            if (this.Features != null)
            {
                if (this.Features.Count == 1)
                {
                    Number1.Text = GetValue(this.Features[0], Number);
                    Start1.Text = GetValue(this.Features[0], Start);
                    End1.Text = GetValue(this.Features[0], End);
                    this.splitContainer1.Panel2.Width = 0;
                }
                else if (this.Features.Count == 2)
                {
                    Number1.Text = GetValue(this.Features[0], Number);
                    Start1.Text = GetValue(this.Features[0], Start);
                    End1.Text = GetValue(this.Features[0], End);
                    Number2.Text = GetValue(this.Features[1], Number);
                    Start2.Text = GetValue(this.Features[1], Start);
                    End2.Text = GetValue(this.Features[1], End);
                }
            }
            else
            {
                MessageBox.Show("未找到相关记录");
            }
        }

        private void OneFeature_Click(object sender, EventArgs e)
        {
            if (this.Features != null)
            {
                if (this.Features.Count >0)
                {
                    AttributeForm form = new AttributeForm(this.Features[0], this.FeatureClass,Father.BusLineName);
                    form.ShowDialog(Father);
                }
            }
        }

        private void TwoFeature_Click(object sender, EventArgs e)
        {
            if (this.Features != null)
            {
                if (this.Features.Count == 2)
                {
                    AttributeForm form = new AttributeForm(this.Features[1], this.FeatureClass, Father.BusLineName);
                    form.ShowDialog(Father);
                }
            }
        }

    }
}
