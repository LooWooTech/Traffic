using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.Models;
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
    public partial class FilterForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private Dictionary<string, int> IndexDict { get; set; }//汉化名称  对应数组序号
        private esriFieldType[] Types { get; set; }//字段类型
        private string[] Fields { get; set; }//字段名称
        public FilterForm(IFeatureClass featureClass)
        {
            InitializeComponent();
            this.FeatureClass = featureClass;
        }
        public FilterForm()
        {
            InitializeComponent();
        }
        private void Init()
        {
            var FieldDict = GISHelper.GetFieldDict(this.FeatureClass);//字段名称  字段类型  所有字段
            var Dict = LayerInfoHelper.GetLayerDictionary(this.FeatureClass.AliasName.GetAlongName());//字段名称  字段名称
            if (FieldDict == null || FieldDict.Count == 0)
            {
                MessageBox.Show("未获取图层相关字段信息.....");
                return;
            }
            var count = FieldDict.Count;
            this.IndexDict = new Dictionary<string, int>();
            this.Types = new esriFieldType[count];
            this.Fields = new string[count];
            int Serial = 0;
            string Label = string.Empty;
            foreach (var key in FieldDict.Keys)
            {
                this.Types[Serial] = FieldDict[key];
                this.Fields[Serial] = key;
                if (Dict.ContainsKey(key))
                {
                    Label = Dict[key];
                }
                else
                {
                    Label = key;
                }
                if (!IndexDict.ContainsKey(Label))
                {
                    IndexDict.Add(Label, Serial);
                }
                Serial++;
            }
            foreach (var key in IndexDict.Keys)
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
        private void SelectedIndexChangedBase(string Values,ConditionNumber Condition)
        {
            if (!string.IsNullOrEmpty(Values))
            {
                if (IndexDict.ContainsKey(Values))
                {
                    var Index = IndexDict[Values];
                    string TypeName = string.Empty;
                    switch (this.Types[Index])
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
                    var unique = GISHelper.GetUniqueValue(this.FeatureClass, this.Fields[Index]).OrderBy(e => e).ToList();
                    switch (Condition)
                    {
                        case ConditionNumber.One:
                            comboBox2.Items.Clear();
                            foreach (var item in list)
                            {
                                comboBox2.Items.Add(item);
                            }
                            comboBox9.Items.Clear();
                            foreach (var item in unique)
                            {
                                comboBox9.Items.Add(item);
                            }
                            break;
                        case ConditionNumber.Two:
                            comboBox3.Items.Clear();
                            foreach (var item in list)
                            {
                                comboBox3.Items.Add(item);
                            }
                            comboBox10.Items.Clear();
                            foreach (var item in unique)
                            {
                                comboBox10.Items.Add(item);
                            }
                            break;
                        case ConditionNumber.Three:
                            comboBox5.Items.Clear();
                            foreach (var item in list)
                            {
                                comboBox5.Items.Add(item);
                            }
                            comboBox11.Items.Clear();
                            foreach (var item in unique)
                            {
                                comboBox11.Items.Add(item);
                            }
                            break;
                    }
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = comboBox1.SelectedItem.ToString();
            Console.WriteLine(val);
            SelectedIndexChangedBase(val, ConditionNumber.One);
            
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = comboBox4.SelectedItem.ToString();
            Console.WriteLine(val);
            SelectedIndexChangedBase(val, ConditionNumber.Two);
        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = comboBox6.SelectedItem.ToString();
            Console.WriteLine(val);
            SelectedIndexChangedBase(val, ConditionNumber.Three);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var WhereClause = string.Empty;
            var values = string.Empty;
            int Index = 0;
            if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null&&comboBox9.Text != null && !string.IsNullOrEmpty(comboBox9.Text)&&IndexDict.ContainsKey(comboBox1.SelectedItem.ToString()))
            {
                Index = IndexDict[comboBox1.SelectedItem.ToString()];
                if (Types[Index] == esriFieldType.esriFieldTypeString)
                {
                    values = "'" + comboBox9.Text + "'";
                }
                else
                {
                    values = comboBox9.Text;
                }
                WhereClause += Fields[Index] + " " + comboBox2.SelectedItem.ToString().GetSQLChar() +" "+ values+" ";
            }
            if (comboBox3.SelectedItem != null && comboBox4.SelectedItem != null &&comboBox7.SelectedItem!=null&&comboBox10.Text!=null&&!string.IsNullOrEmpty(comboBox10.Text)&&IndexDict.ContainsKey(comboBox4.SelectedItem.ToString()))
            {
                Index = IndexDict[comboBox4.SelectedItem.ToString()];
                if (Types[Index] == esriFieldType.esriFieldTypeString)
                {
                    values = "'" + comboBox10.Text + "'";
                }
                else
                {
                    values = comboBox10.Text;
                }
                WhereClause += comboBox7.SelectedItem.ToString().GetSQLChar() + " " + Fields[Index] + " " + comboBox3.SelectedItem.ToString().GetSQLChar() + " " + comboBox10.Text+" ";
            }
            if (comboBox5.SelectedItem != null && comboBox6.SelectedItem != null &&comboBox8.SelectedItem!=null&&comboBox11.Text!=null&&!string.IsNullOrEmpty(comboBox11.Text)&&IndexDict.ContainsKey(comboBox6.SelectedItem.ToString()))
            {
                Index = IndexDict[comboBox6.SelectedItem.ToString()];
                if (Types[Index] == esriFieldType.esriFieldTypeString)
                {
                    values = "'" + comboBox11.Text + "'";
                }
                else
                {
                    values = comboBox11.Text;
                }
                WhereClause += comboBox8.SelectedItem.ToString().GetSQLChar() +" "+ Fields[Index] +" "+ comboBox5.SelectedItem.ToString().GetSQLChar() +" "+ comboBox11.Text+" ";
            }
            MainForm form1 = (MainForm)this.Owner;
            form1.toolStripStatusLabel1.Text = WhereClause;
            form1.ConditionControlCenter();
            this.Close();
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
