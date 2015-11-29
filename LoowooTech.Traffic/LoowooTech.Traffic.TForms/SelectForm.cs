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
        public SelectForm()
        {
            InitializeComponent();
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            Father = this.Owner as MainForm;
            foreach (StatisticMode mode in Enum.GetValues(typeof(StatisticMode)))
            {
                comboBox1.Items.Add(mode.GetDescription());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                Father.StatisticBase(comboBox1.SelectedItem.ToString());
            }
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
