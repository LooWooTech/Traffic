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
    public partial class BusStopForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private string WhereClause { get; set; }
        private Form1 Father { get; set; }
        private Dictionary<int, IFeature> FeatureDict { get; set; }
        public BusStopForm(IFeatureClass FeatureClass,string WhereClause)
        {
            InitializeComponent();
            this.FeatureClass = FeatureClass;
            this.WhereClause = WhereClause;
        }

        private void BusStopForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("进入load");
            this.Father = (Form1)this.Owner;
            try
            {
                var list = GISHelper.Search(FeatureClass, WhereClause);
                var tempdict = GISHelper.GetFieldIndexDict(FeatureClass);
                Dictionary<int, IFeature> temp;
                dataGridView1.DataSource = AttributeHelper.GetTable(list, tempdict, out temp);
                if (temp != null)
                {
                    FeatureDict = temp;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            
        }

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

    }
}
