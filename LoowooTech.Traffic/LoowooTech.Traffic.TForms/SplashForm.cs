﻿using LoowooTech.Traffic.Manager;
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
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }

        public MainForm Form { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Text = "正在登陆";
            this.button1.Enabled = false;
            var tool = new UserManager();
            var user = tool.Login(this.textBox1.Text, this.textBox2.Text);
            if (user != null)
            {
                if (Form != null)
                {
                    Form.Enabled = true;
                    Form.CurrentUser = user;
                    Form.Power();
                }
                    
                MessageBox.Show("成功登陆"+user.Role.GetDescription().Trim()+" 用户:"+user.Name.Trim());
                this.Close();
            }
            else
            {
                MessageBox.Show("用户名或者密码错误，请核对！");
            }
            this.button1.Text = "登陆";
            this.button1.Enabled = true;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(sender, e);
            }
        }
    }
}
