using LoowooTech.Traffic.Models;
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
    public partial class SelectForm : Form
    {
        private MainForm Father { get; set; }
        private List<string> ValuesList { get; set; }
        public SelectForm()
        {
            InitializeComponent();
        }
        public SelectForm(List<string> List)
        {
            InitializeComponent();
            this.ValuesList = List;
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            Father = this.Owner as MainForm;
            foreach (var item in this.ValuesList)
            {
                comboBox1.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                if (Father.inquiryMode == InquiryMode.Statistic)
                {
                    Father.StatisticBase(comboBox1.SelectedItem.ToString());
                }
                
            }
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
