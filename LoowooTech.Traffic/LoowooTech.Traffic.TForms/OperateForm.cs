using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
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
    public partial class OperateForm : Form
    {
        private IFeatureClass FeatureClass { get; set; }
        private Dictionary<string,int> FieldIndexDict { get; set; } //字段名称   对应  Index
        private Dictionary<string, int> RelationDict { get; set; }
        private Dictionary<string, string> Dict { get; set; }
        private IGeometry geometry { get; set; }
        private IFeature Feature { get; set; }
        private MainForm Father { get; set; } 
        private Label[] Labels { get; set; }
        private TextBox[] TextBoxs { get; set; }
       
        public OperateForm(IFeatureClass FeatureClass, IGeometry geometry)
        {
            this.FeatureClass = FeatureClass;
            this.FieldIndexDict = GISHelper.GetFieldIndexDict(FeatureClass);
            this.geometry = geometry;
            Init();
            InitializeComponent();
        }

        public OperateForm(IFeatureClass FeatureClass, IFeature Feature)
        {
            this.FeatureClass = FeatureClass;
            this.Feature = Feature;
            this.FieldIndexDict = GISHelper.GetFieldIndexDict(FeatureClass);
            Init();
            InitializeComponent();
        }

        public OperateForm()
        {
            InitializeComponent();
        }
        private void InitControl(int Index,int Width,string FieldName,string Label,string Values=null)
        {
            this.Labels[Index] = new Label();
            this.Labels[Index].Location = new System.Drawing.Point(20,30+Index*Width);
            this.Labels[Index].Name = "label" + FieldName;
            this.Labels[Index].Size = new System.Drawing.Size(80, 12);
            this.Labels[Index].Text = Label;
            this.Controls.Add(this.Labels[Index]);

            this.TextBoxs[Index] = new TextBox();
            this.TextBoxs[Index].Location = new System.Drawing.Point(120, 30 + Index*Width);
            this.TextBoxs[Index].Multiline = true;
            this.TextBoxs[Index].Name = FieldName;
            this.TextBoxs[Index].Size = new System.Drawing.Size(150, 21);
            this.TextBoxs[Index].Text = Values;
            this.Controls.Add(this.TextBoxs[Index]);
        }

        private void Init()
        {
            this.Labels = new Label[FieldIndexDict.Count];
            this.TextBoxs = new TextBox[FieldIndexDict.Count];
            this.RelationDict = new Dictionary<string, int>();
            this.Dict = LayerInfoHelper.GetLayerDictionary(FeatureClass.AliasName.GetAlongName());
            Dictionary<string,string> Temp=null;
            if (Feature != null)
            {
                Temp = AttributeHelper.GetValues(this.Feature, FieldIndexDict);
            }
            int serial = 0;
            string label = string.Empty;
            foreach (var key in FieldIndexDict.Keys)
            {
                if (key.ToUpper() == "SHAPE" || key.ToUpper() == "OBJECTID"||key.ToUpper().Contains("OBJECTID")||key.ToUpper().Contains("SHAPE")||key.ToUpper()=="NO_")
                {
                    continue;
                }
                if (Dict.ContainsKey(key))
                {
                    label = Dict[key];
                }
                else
                {
                    label = key;
                }
                if(Temp!=null&&Temp.ContainsKey(key))
                {
                    InitControl(serial, 40, key, label, Temp[key]);
                }
                else
                {
                    InitControl(serial, 40, key, label);
                }
                
                RelationDict.Add(key, serial);
                serial++;
            }
        }

        private Dictionary<string, string> GetFieldValue()
        {
            var dict = new Dictionary<string, string>();
            var count = this.TextBoxs.Count();
            for (var i = 0; i < count; i++)
            {
                if (this.TextBoxs[i]!=null &&this.TextBoxs[i].Name != null && this.TextBoxs[i].Text != null)
                {
                    dict.Add(this.TextBoxs[i].Name, this.TextBoxs[i].Text);
                }
                
            }    
            return dict;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var val = GetFieldValue();
            if (Father.operateMode == OperateMode.Add)
            {
                if (!SDEManager.AddFeature(val, FieldIndexDict, FeatureClass, geometry))
                {
                    MessageBox.Show("添加失败！");
                }
            }
            else if (Father.operateMode == OperateMode.Edit)
            {
                SDEManager.EditFeature(val, FieldIndexDict, this.Feature);
            }
            
            Father.MapRefresh();
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OperateForm_Load(object sender, EventArgs e)
        {
            this.Father = (MainForm)this.Owner;
        }
    }
}
