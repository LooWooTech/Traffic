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
        public BusFilterForm()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                Father.BusLineWhereClause = string.Empty;
                Father.BusStopWhereClause = string.Empty;
            }
            else
            {
                Father.BusLineWhereClause = "nameshort='" + textBox1.Text + "'";
                Father.BusStopWhereClause = "stopName='" + textBox1.Text + "'";
            }
            Father.UpdateBus();
            Father.ShowBus();
            this.Close();
        }

        private void BusFilterForm_Load(object sender, EventArgs e)
        {
            this.Father = (MainForm)this.Owner;
        }


    }
}
