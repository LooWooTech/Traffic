namespace LoowooTech.Traffic.TForms
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.路网ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportActiveView = new System.Windows.Forms.ToolStripMenuItem();
            this.RoadFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchButton = new System.Windows.Forms.ToolStripMenuItem();
            this.PointSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.ConditionSearchButton = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            this.axMapControl1 = new ESRI.ArcGIS.Controls.AxMapControl();
            this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.ExportSHP = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 605);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(941, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.路网ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(941, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 路网ToolStripMenuItem
            // 
            this.路网ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportActiveView,
            this.RoadFilter,
            this.SearchButton,
            this.ExportSHP,
            this.ExportExcel});
            this.路网ToolStripMenuItem.Name = "路网ToolStripMenuItem";
            this.路网ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.路网ToolStripMenuItem.Text = "路网";
            // 
            // ExportActiveView
            // 
            this.ExportActiveView.Name = "ExportActiveView";
            this.ExportActiveView.Size = new System.Drawing.Size(152, 22);
            this.ExportActiveView.Text = "导出图片";
            this.ExportActiveView.Click += new System.EventHandler(this.ExportActiveView_Click);
            // 
            // RoadFilter
            // 
            this.RoadFilter.Name = "RoadFilter";
            this.RoadFilter.Size = new System.Drawing.Size(152, 22);
            this.RoadFilter.Text = "路网过滤";
            this.RoadFilter.Click += new System.EventHandler(this.RoadFilter_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PointSearch,
            this.ConditionSearchButton});
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(152, 22);
            this.SearchButton.Text = "查询";
            // 
            // PointSearch
            // 
            this.PointSearch.Name = "PointSearch";
            this.PointSearch.Size = new System.Drawing.Size(124, 22);
            this.PointSearch.Text = "点选查询";
            this.PointSearch.Click += new System.EventHandler(this.PointSearch_Click);
            // 
            // ConditionSearchButton
            // 
            this.ConditionSearchButton.Name = "ConditionSearchButton";
            this.ConditionSearchButton.Size = new System.Drawing.Size(124, 22);
            this.ConditionSearchButton.Text = "条件查询";
            this.ConditionSearchButton.Click += new System.EventHandler(this.ConditionSearchButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 53);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.axTOCControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.axLicenseControl1);
            this.splitContainer1.Panel2.Controls.Add(this.axMapControl1);
            this.splitContainer1.Size = new System.Drawing.Size(941, 552);
            this.splitContainer1.SplitterDistance = 172;
            this.splitContainer1.TabIndex = 4;
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axTOCControl1.Location = new System.Drawing.Point(0, 0);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(172, 552);
            this.axTOCControl1.TabIndex = 0;
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(721, 521);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 1;
            // 
            // axMapControl1
            // 
            this.axMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMapControl1.Location = new System.Drawing.Point(0, 0);
            this.axMapControl1.Name = "axMapControl1";
            this.axMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl1.OcxState")));
            this.axMapControl1.Size = new System.Drawing.Size(765, 552);
            this.axMapControl1.TabIndex = 0;
            this.axMapControl1.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.axMapControl1_OnMouseDown);
            // 
            // axToolbarControl1
            // 
            this.axToolbarControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.axToolbarControl1.Location = new System.Drawing.Point(0, 25);
            this.axToolbarControl1.Name = "axToolbarControl1";
            this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
            this.axToolbarControl1.Size = new System.Drawing.Size(941, 28);
            this.axToolbarControl1.TabIndex = 3;
            // 
            // ExportSHP
            // 
            this.ExportSHP.Name = "ExportSHP";
            this.ExportSHP.Size = new System.Drawing.Size(152, 22);
            this.ExportSHP.Text = "导出SHP";
            this.ExportSHP.Click += new System.EventHandler(this.ExportSHP_Click);
            // 
            // ExportExcel
            // 
            this.ExportExcel.Name = "ExportExcel";
            this.ExportExcel.Size = new System.Drawing.Size(152, 22);
            this.ExportExcel.Text = "导出Excel";
            this.ExportExcel.Click += new System.EventHandler(this.ExportExcel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 627);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.axToolbarControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        public  ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        public System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
        private System.Windows.Forms.ToolStripMenuItem 路网ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExportActiveView;
        private System.Windows.Forms.ToolStripMenuItem RoadFilter;
        private System.Windows.Forms.ToolStripMenuItem SearchButton;
        private System.Windows.Forms.ToolStripMenuItem PointSearch;
        private System.Windows.Forms.ToolStripMenuItem ConditionSearchButton;
        private System.Windows.Forms.ToolStripMenuItem ExportSHP;
        private System.Windows.Forms.ToolStripMenuItem ExportExcel;


    }
}

