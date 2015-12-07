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
    public partial class HistoryForm : Form
    {
        public HistoryForm()
        {
            InitializeComponent();

            var item = new ListViewItem(new[] { "1", "3538" , "3773", "2015-09-01 12:05:01" });
            lstResult.Items.Add(item);

            var item2 = new ListViewItem(new[] { "1", "3225", "3538", "2014-8-10 08:12:45" });
            lstResult.Items.Add(item2);

        }
    }
}
