namespace LoowooTech.Traffic.TForms
{
    partial class UserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserForm));
            this.ribbon1 = new System.Windows.Forms.Ribbon();
            this.ribbonTab1 = new System.Windows.Forms.RibbonTab();
            this.ribbonPanel1 = new System.Windows.Forms.RibbonPanel();
            this.AddUser = new System.Windows.Forms.RibbonButton();
            this.EditUser = new System.Windows.Forms.RibbonButton();
            this.DeleteUser = new System.Windows.Forms.RibbonButton();
            this.ribbonPanel2 = new System.Windows.Forms.RibbonPanel();
            this.btnClose = new System.Windows.Forms.RibbonButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon1
            // 
            this.ribbon1.CaptionBarVisible = false;
            this.ribbon1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.ribbon1.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.Minimized = false;
            this.ribbon1.Name = "ribbon1";
            // 
            // 
            // 
            this.ribbon1.OrbDropDown.BorderRoundness = 8;
            this.ribbon1.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.OrbDropDown.Name = "";
            this.ribbon1.OrbDropDown.Size = new System.Drawing.Size(527, 447);
            this.ribbon1.OrbDropDown.TabIndex = 0;
            this.ribbon1.OrbImage = null;
            this.ribbon1.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2010;
            this.ribbon1.OrbVisible = false;
            this.ribbon1.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
            this.ribbon1.Size = new System.Drawing.Size(574, 117);
            this.ribbon1.TabIndex = 1;
            this.ribbon1.Tabs.Add(this.ribbonTab1);
            this.ribbon1.TabsMargin = new System.Windows.Forms.Padding(12, 2, 20, 0);
            this.ribbon1.Text = "ribbon1";
            this.ribbon1.ThemeColor = System.Windows.Forms.RibbonTheme.Blue;
            // 
            // ribbonTab1
            // 
            this.ribbonTab1.Panels.Add(this.ribbonPanel1);
            this.ribbonTab1.Panels.Add(this.ribbonPanel2);
            this.ribbonTab1.Text = "功能";
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.Items.Add(this.AddUser);
            this.ribbonPanel1.Items.Add(this.EditUser);
            this.ribbonPanel1.Items.Add(this.DeleteUser);
            this.ribbonPanel1.Text = "";
            // 
            // AddUser
            // 
            this.AddUser.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.AssignTo_32x32;
            this.AddUser.SmallImage = ((System.Drawing.Image)(resources.GetObject("AddUser.SmallImage")));
            this.AddUser.Text = "添加用户";
            this.AddUser.Click += new System.EventHandler(this.AddUser_Click);
            // 
            // EditUser
            // 
            this.EditUser.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Edit_32x32;
            this.EditUser.SmallImage = ((System.Drawing.Image)(resources.GetObject("EditUser.SmallImage")));
            this.EditUser.Text = "修改用户权限";
            this.EditUser.Click += new System.EventHandler(this.EditUser_Click);
            // 
            // DeleteUser
            // 
            this.DeleteUser.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Delete_32x32;
            this.DeleteUser.SmallImage = ((System.Drawing.Image)(resources.GetObject("DeleteUser.SmallImage")));
            this.DeleteUser.Text = "删除用户";
            this.DeleteUser.Click += new System.EventHandler(this.DeleteUser_Click);
            // 
            // ribbonPanel2
            // 
            this.ribbonPanel2.Items.Add(this.btnClose);
            this.ribbonPanel2.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Close_32x32;
            this.btnClose.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnClose.SmallImage")));
            this.btnClose.Text = "关闭窗口";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 117);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(574, 277);
            this.dataGridView1.TabIndex = 2;
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 394);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.ribbon1);
            this.Name = "UserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "用户管理";
            this.Load += new System.EventHandler(this.UserForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Ribbon ribbon1;
        private System.Windows.Forms.RibbonTab ribbonTab1;
        private System.Windows.Forms.RibbonPanel ribbonPanel1;
        private System.Windows.Forms.RibbonButton AddUser;
        private System.Windows.Forms.RibbonButton EditUser;
        private System.Windows.Forms.RibbonButton DeleteUser;
        private System.Windows.Forms.RibbonPanel ribbonPanel2;
        private System.Windows.Forms.RibbonButton btnClose;
        private System.Windows.Forms.DataGridView dataGridView1;

    }
}