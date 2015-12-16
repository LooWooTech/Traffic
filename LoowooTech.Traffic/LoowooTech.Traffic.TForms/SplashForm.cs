using LoowooTech.Traffic.Manager;
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
                    
                MessageBox.Show("成功登陆"+user.Role.GetDescription()+" 用户:"+user.Name);
                this.Close();
            }
            else
            {
                MessageBox.Show("用户名或者密码错误，请核对！");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
