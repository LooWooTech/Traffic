using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.Manager;
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
    public partial class UserForm : Form
    {
        private List<User> List { get; set; }
        public UserForm()
        {
            InitializeComponent();
            var tool=new UserManager();
            this.List = tool.GetList();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = AttributeHelper.GetTable(this.List);
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].Value.ToString();
                MessageBox.Show(selectIndex);
            }
        }
    }
}
