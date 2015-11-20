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
            if (!string.IsNullOrEmpty(WhereClause) && FeatureClass != null)
            {
                dataGridView1.DataSource = AttributeHelper.GetTable(FeatureClass, WhereClause);
            }
        }
    }
}
