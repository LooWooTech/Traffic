using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoowooTech.Traffic.Models;
using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.Manager;

namespace LoowooTech.Traffic.TForms
{
    public partial class ChangePasswordForm : Form
    {
        private User _user { get; set; }
        private UserManager _tool { get; set; }
        private User _CurrentUser { get; set; }
        private bool _flag { get; set; }
        public ChangePasswordForm(User user,bool flag=false)
        {
            InitializeComponent();
            _user = user;
            _CurrentUser = user;
            _flag = flag;
            _tool = new UserManager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_flag)
            {
                var str = textBox1.Text.MD5().Trim();
                if (str != _CurrentUser.Password.Trim())
                {
                    MessageBox.Show("原始密码不正确");
                    return;
                }
            }
            
            if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("输入的两次密码不一致");
                return;
            }
            _user = _CurrentUser;
            _user.Password = textBox2.Text.MD5();
            try
            {
                _tool.Edit(_user);
                MessageBox.Show("成功修改密码,重启程序生效！");
                this.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            if (_flag)
            {
                this.label1.Visible = false;
                this.textBox1.Visible = false;
                this.label4.Visible = true;
                this.label4.Text = string.Format("当前重置{0}的密码",_CurrentUser.Name);
            }
            else
            {
                this.label4.Visible = false;
            }
        }
    }
}
