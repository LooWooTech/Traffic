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
    public partial class BusFilterForm : Form
    {
        private MainForm Father { get; set; }
        private IFeatureClass FeatureClass { get; set; }
        private List<string> Values { get; set; }
        public BusFilterForm()
        {
            InitializeComponent();
           
        }
        public BusFilterForm(List<string> List,IFeatureClass FeatureClass)
        {
            InitializeComponent();
            this.Values = List;
            this.FeatureClass = FeatureClass;
        }
        private void Init()
        {
            foreach (var item in this.Values)
            {
                comboBox1.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != null && !string.IsNullOrEmpty(comboBox1.Text))
            {
                ChooseForm chooseForm = new ChooseForm(this.FeatureClass, System.Configuration.ConfigurationManager.AppSettings["BUSKEY"] + " = '" + comboBox1.Text + "'",Father);
                chooseForm.ShowDialog();
            }
        }

        private void BusFilterForm_Load(object sender, EventArgs e)
        {
            this.Father = (MainForm)this.Owner;
            Init();
        }


    }
}
