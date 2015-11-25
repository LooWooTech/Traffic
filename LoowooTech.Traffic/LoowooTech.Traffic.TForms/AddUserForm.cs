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
using LoowooTech.Traffic.Manager;

namespace LoowooTech.Traffic.TForms
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

        private void AddUserForm_Load(object sender, EventArgs e)
        {
            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                comboBox1.Items.Add(role.GetDescription());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("用户名或者密码为空");
                return;
            }
            if (comboBox1.SelectedItem != null)
            {
                Role role = GetRole(comboBox1.SelectedItem.ToString());
                var Tool = new UserManager();
                Tool.Add(new User()
                {
                    Name = textBox1.Text,
                    Password = textBox2.Text,
                    Role = role
                });
                MessageBox.Show("成功添加用户");
                this.Close();
            }
            else
            {
                MessageBox.Show("权限组未选择！");
            }
            
           

        }

        private Role GetRole(string Description)
        {
            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                if (role.GetDescription() == Description)
                {
                    return role;
                }
            }
            return Role.Common;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
