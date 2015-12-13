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
                this.textBox1.AutoCompleteCustomSource.Add(item);
            }
            this.textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null && !string.IsNullOrEmpty(textBox1.Text))
            {
                string whereClause = System.Configuration.ConfigurationManager.AppSettings["BUSKEY"] + " = '" + textBox1.Text + "'";
                string StopWhereClause = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY1"] + " = " + textBox1.Text.Replace("路", "").Replace("区间","").Replace("线","").Replace("高峰大站车","");
                Father.UpdateBase(Father.BusLineName, whereClause, FeatureClass);
                Father.UpdateBase(Father.BusStopName, StopWhereClause,Father.BusStopFeatureClass);
                ChooseForm chooseForm = new ChooseForm(this.FeatureClass, whereClause,Father);
                chooseForm.ShowDialog();
            }
        }

        private void BusFilterForm_Load(object sender, EventArgs e)
        {
            this.Father = (MainForm)this.Owner;
            Init();
        }

        private void BusFilterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Father.UpdateBase(Father.BusLineName, "", FeatureClass);
            Father.UpdateBase(Father.BusStopName, "", Father.BusStopFeatureClass);
        }


    }
}
