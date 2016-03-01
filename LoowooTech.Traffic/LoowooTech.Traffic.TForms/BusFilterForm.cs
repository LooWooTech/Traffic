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
                var list = GISHelper.GetRoadList(Father.BusLineFeatureClass, Father.BusStopFeatureClass, textBox1.Text);
                if (list.Count > 0)
                {
                    Father.UpdateBase(Father.BusLineName, list[0].RoadWhereClause, Father.BusLineFeatureClass,true,true);
                    Father.UpdateBase(Father.BusStopName, list[0].StopWhereClause, Father.BusStopFeatureClass,false,true);
                    Father.UpdateStartEnd(list, Father.StartEndName, Father.StartEndFeatureClass);
                    ChooseForm chooseForm = new ChooseForm(list, Father);
                    chooseForm.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("当前搜索结果为空");
                    return;
                }
                //string whereClause = System.Configuration.ConfigurationManager.AppSettings["BUSKEY"] + " = '" + textBox1.Text + "'";
                ////var dict = GISHelper.GetWhereClauseFeature(FeatureClass, System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY2"], whereClause);
                //string StopWhereClause = System.Configuration.ConfigurationManager.AppSettings["BUSSTOPKEY1"] + " = " + textBox1.Text.Replace("路", "").Replace("区间","").Replace("线","").Replace("高峰大站车","");
                //Father.UpdateBase(Father.BusLineName, whereClause, FeatureClass);
                //Father.UpdateBase(Father.BusStopName, StopWhereClause,Father.BusStopFeatureClass);
               
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(sender, e);
            }
        }


    }
}
