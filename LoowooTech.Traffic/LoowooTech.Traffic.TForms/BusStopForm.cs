using ESRI.ArcGIS.Geodatabase;
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
    public partial class BusStopForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private string WhereClause { get; set; }
        private Form1 Father { get; set; }
        public BusStopForm(IFeatureClass FeatureClass,string WhereClause)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.WhereClause = WhereClause;
        }

        private void BusStopForm_Load(object sender, EventArgs e)
        {
            this.Father = (Form1)this.Owner;
        }

    }
}
