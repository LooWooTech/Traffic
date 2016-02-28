namespace LoowooTech.Traffic.TForms
{
    partial class HistoryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryForm));
            this.ribbon1 = new System.Windows.Forms.Ribbon();
            this.ribbonTab1 = new System.Windows.Forms.RibbonTab();
            this.ribbonPanel1 = new System.Windows.Forms.RibbonPanel();
            this.btnFlash = new System.Windows.Forms.RibbonButton();
            this.btnZoom = new System.Windows.Forms.RibbonButton();
            this.ribbonSeparator1 = new System.Windows.Forms.RibbonSeparator();
            this.btnRecover = new System.Windows.Forms.RibbonButton();
            this.ribbonPanel2 = new System.Windows.Forms.RibbonPanel();
            this.btnClose = new System.Windows.Forms.RibbonButton();
            this.lstResult = new System.Windows.Forms.ListView();
            this.序号 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.新道路编号 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.新道路名称 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.原道路编号 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.ribbon1.Size = new System.Drawing.Size(634, 114);
            this.ribbon1.TabIndex = 2;
            this.ribbon1.Tabs.Add(this.ribbonTab1);
            this.ribbon1.TabsMargin = new System.Windows.Forms.Padding(12, 2, 20, 0);
            this.ribbon1.Text = "ribbon1";
            this.ribbon1.ThemeColor = System.Windows.Forms.RibbonTheme.Blue;
            // 
            // ribbonTab1
            // 
            this.ribbonTab1.Panels.Add(this.ribbonPanel1);
            this.ribbonTab1.Panels.Add(this.ribbonPanel2);
            this.ribbonTab1.Text = "操作";
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.Items.Add(this.btnFlash);
            this.ribbonPanel1.Items.Add(this.btnZoom);
            this.ribbonPanel1.Items.Add(this.ribbonSeparator1);
            this.ribbonPanel1.Items.Add(this.btnRecover);
            this.ribbonPanel1.Text = "";
            // 
            // btnFlash
            // 
            this.btnFlash.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Wizard_32x32;
            this.btnFlash.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnFlash.SmallImage")));
            this.btnFlash.Text = "闪烁道路";
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
            // 
            // btnZoom
            // 
            this.btnZoom.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Zoom2_32x32;
            this.btnZoom.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnZoom.SmallImage")));
            this.btnZoom.Text = "移动到道路";
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // btnRecover
            // 
            this.btnRecover.Image = global::LoowooTech.Traffic.TForms.Properties.Resources._1449523215_URL_History_64;
            this.btnRecover.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnRecover.SmallImage")));
            this.btnRecover.Text = "退回到";
            this.btnRecover.Click += new System.EventHandler(this.btnRecover_Click);
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
            // lstResult
            // 
            this.lstResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.序号,
            this.新道路编号,
            this.新道路名称,
            this.原道路编号});
            this.lstResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstResult.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstResult.FullRowSelect = true;
            this.lstResult.GridLines = true;
            this.lstResult.Location = new System.Drawing.Point(0, 114);
            this.lstResult.MultiSelect = false;
            this.lstResult.Name = "lstResult";
            this.lstResult.ShowGroups = false;
            this.lstResult.Size = new System.Drawing.Size(634, 276);
            this.lstResult.TabIndex = 3;
            this.lstResult.UseCompatibleStateImageBehavior = false;
            this.lstResult.View = System.Windows.Forms.View.Details;
            // 
            // 序号
            // 
            this.序号.Text = "序号";
            // 
            // 新道路编号
            // 
            this.新道路编号.Text = "原道路编号";
            this.新道路编号.Width = 137;
            // 
            // 新道路名称
            // 
            this.新道路名称.Text = "新道路编号";
            this.新道路名称.Width = 142;
            // 
            // 原道路编号
            // 
            this.原道路编号.Text = "归档时间";
            this.原道路编号.Width = 155;
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 390);
            this.Controls.Add(this.lstResult);
            this.Controls.Add(this.ribbon1);
            this.Name = "HistoryForm";
            this.Text = "道路历史信息";
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Ribbon ribbon1;
        private System.Windows.Forms.RibbonTab ribbonTab1;
        private System.Windows.Forms.RibbonPanel ribbonPanel1;
        private System.Windows.Forms.RibbonButton btnFlash;
        private System.Windows.Forms.RibbonButton btnZoom;
        private System.Windows.Forms.RibbonSeparator ribbonSeparator1;
        private System.Windows.Forms.RibbonButton btnRecover;
        private System.Windows.Forms.RibbonPanel ribbonPanel2;
        private System.Windows.Forms.RibbonButton btnClose;
        private System.Windows.Forms.ListView lstResult;
        private System.Windows.Forms.ColumnHeader 序号;
        private System.Windows.Forms.ColumnHeader 新道路编号;
        private System.Windows.Forms.ColumnHeader 新道路名称;
        private System.Windows.Forms.ColumnHeader 原道路编号;

    }
}