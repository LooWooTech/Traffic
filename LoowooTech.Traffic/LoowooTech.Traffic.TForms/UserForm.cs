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
        private UserManager Tool { get; set; }
        public UserForm()
        {
            InitializeComponent();
            this.Tool=new UserManager();
            this.List = Tool.GetList();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.dataGridView1.DataSource = AttributeHelper.GetTable(this.List);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].Value.ToString();
                MessageBox.Show(selectIndex);
            }
        }

        private void AddUser_Click(object sender, EventArgs e)
        {
            AddUserForm form = new AddUserForm();
            form.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int GetID()
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                var selectIndex = dataGridView1.SelectedCells[0].RowIndex;
                var Table = dataGridView1.DataSource as DataTable;
                int ID = 0;
                if (int.TryParse(Table.Rows[selectIndex][0].ToString(), out ID))
                {
                    return ID;
                }
                return -1;
            }
            return -1;
        }

        private void DeleteUser_Click(object sender, EventArgs e)
        {
            var ID = GetID();
            if (ID != -1)
            {
                var user = Tool.Search(ID);
                if (user != null)
                {
                    if (MessageBox.Show("您确定要删除" + user.Name, "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (Tool.Delete(ID))
                        {
                            MessageBox.Show("删除用户成功！");
                        }
                        else
                        {
                            MessageBox.Show("删除失败！");
                        }
                    }
                }
            }
        }

        private void EditUser_Click(object sender, EventArgs e)
        {
            var ID = GetID();
            if (ID != -1)
            {
                var user = Tool.Search(ID);
                if (user != null)
                {
                    AddUserForm form = new AddUserForm(user);
                    form.ShowDialog(this);
                }
            }
        }

        private void ReSetPassword_Click(object sender, EventArgs e)
        {
            var ID = GetID();
            if (ID != -1)
            {
                var user = Tool.Search(ID);
                if (user.Name.Trim().ToUpper() == "Admin".Trim().ToUpper())
                {
                    MessageBox.Show("当前禁止重置超级管理员密码");
                    return;
                }
                var form = new ChangePasswordForm(user, true);
                form.ShowDialog();
            }
        }
    }
}
