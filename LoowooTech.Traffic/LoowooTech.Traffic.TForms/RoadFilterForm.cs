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
    public partial class RoadFilterForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private Dictionary<string, esriFieldType> FieldDict { get; set; }
        public RoadFilterForm(IFeatureClass featureClass)
        {
            InitializeComponent();
            this.FeatureClass = featureClass;
            this.FieldDict = GISHelper.GetFieldDict(featureClass);
        }
        public RoadFilterForm()
        {
            InitializeComponent();
        }

        private void Init()
        {
            if (FieldDict == null || FieldDict.Count == 0)
            {
                MessageBox.Show("未获取相关字段信息....");
                return;
            }
            foreach (var key in FieldDict.Keys)
            {
                comboBox1.Items.Add(key);
                comboBox4.Items.Add(key);
                comboBox6.Items.Add(key);
            }
        }

        private void RoadFilterForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = comboBox1.SelectedItem.ToString();
            Console.WriteLine(val);
            if (!string.IsNullOrEmpty(val))
            {
                if (FieldDict.ContainsKey(val))
                {
                    var fieldType = FieldDict[val];
                    comboBox2.Items.Clear();
                    string TypeName = string.Empty;
                    switch (fieldType)
                    {
                        case esriFieldType.esriFieldTypeString:
                            TypeName = "String";
                            break;
                        case esriFieldType.esriFieldTypeDouble:
                        case esriFieldType.esriFieldTypeInteger:
                        case esriFieldType.esriFieldTypeOID:
                        case esriFieldType.esriFieldTypeSmallInteger:
                        case esriFieldType.esriFieldTypeGUID:
                        case esriFieldType.esriFieldTypeGlobalID:
                            TypeName = "Int";
                            break;
                        default:
                            TypeName = "String";
                            break;
                    }
                    var list = RelationHelper.GetRelations(TypeName);
                    foreach (var item in list)
                    {
                        comboBox2.Items.Add(item);
                    }
                }
            }
            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = comboBox4.SelectedItem.ToString();
            Console.WriteLine(val);
            if (!string.IsNullOrEmpty(val))
            {
                if (FieldDict.ContainsKey(val))
                {
                    var fieldType = FieldDict[val];
                    comboBox3.Items.Clear();
                    string TypeName = string.Empty;
                    switch (fieldType)
                    {
                        case esriFieldType.esriFieldTypeString:
                            TypeName = "String";
                            break;
                        case esriFieldType.esriFieldTypeDouble:
                        case esriFieldType.esriFieldTypeInteger:
                        case esriFieldType.esriFieldTypeGlobalID:
                        case esriFieldType.esriFieldTypeGUID:
                        case esriFieldType.esriFieldTypeOID:
                        case esriFieldType.esriFieldTypeSmallInteger:
                            TypeName = "Int";
                            break;
                        default :
                            TypeName = "String";
                            break;
                    }
                    var list = RelationHelper.GetRelations(TypeName);
                    foreach (var item in list)
                    {
                        comboBox3.Items.Add(item);
                    }
                }
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = comboBox6.SelectedItem.ToString();
            Console.WriteLine(val);
            if (!string.IsNullOrEmpty(val))
            {
                if (FieldDict.ContainsKey(val))
                {
                    var fieldType = FieldDict[val];
                    comboBox5.Items.Clear();
                    string TypeName = string.Empty;
                    switch (fieldType)
                    {
                        case esriFieldType.esriFieldTypeString:
                            TypeName = "String";
                            break;
                        case esriFieldType.esriFieldTypeDouble:
                        case esriFieldType.esriFieldTypeInteger:
                        case esriFieldType.esriFieldTypeSmallInteger:
                        case esriFieldType.esriFieldTypeOID:
                        case esriFieldType.esriFieldTypeGUID:
                        case esriFieldType.esriFieldTypeGlobalID:
                            TypeName = "Int";
                            break;
                        default:
                            TypeName = "String";
                            break;
                    }
                    var list = RelationHelper.GetRelations(TypeName);
                    foreach (var item in list)
                    {
                        comboBox5.Items.Add(item);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var WhereClause = string.Empty;
            if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null && textBox1.Text != null)
            {
                WhereClause += comboBox1.SelectedItem.ToString() +" "+ comboBox2.SelectedItem.ToString().GetSQLChar() +" '"+ textBox1.Text+"' ";
            }
            if (comboBox3.SelectedItem != null && comboBox4.SelectedItem != null && textBox2.Text != null&&comboBox7.SelectedItem!=null)
            {
                WhereClause += comboBox7.SelectedItem.ToString().GetSQLChar() +" "+ comboBox4.SelectedItem.ToString()+" " + comboBox3.SelectedItem.ToString().GetSQLChar() +" '"+ textBox2.Text+"' ";
            }
            if (comboBox5.SelectedItem != null && comboBox6.SelectedItem != null && textBox3.Text != null&&comboBox8.SelectedItem!=null)
            {
                WhereClause += comboBox8.SelectedItem.ToString().GetSQLChar() +" "+ comboBox6.SelectedItem.ToString() +" "+ comboBox5.SelectedItem.ToString().GetSQLChar() +" '"+ textBox3.Text+"' ";
            }
            Form1 form1 = (Form1)this.Owner;
            form1.toolStripStatusLabel1.Text = WhereClause;
            switch (form1.roadMode)
            {
                case Models.RoadMode.Filter:
                    form1.UpdateRoad();
                    break;
                case Models.RoadMode.Search:
                    form1.ShowResult();
                    break;
            }
            this.Close();
            
            
        }
    }
}
