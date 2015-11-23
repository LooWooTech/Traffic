namespace LoowooTech.Traffic.TForms
{
    partial class BusResultForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ViewButton = new System.Windows.Forms.Button();
            this.ExportExcel = new System.Windows.Forms.Button();
            this.TwinkleButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ViewButton);
            this.splitContainer1.Panel1.Controls.Add(this.ExportExcel);
            this.splitContainer1.Panel1.Controls.Add(this.TwinkleButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(441, 265);
            this.splitContainer1.SplitterDistance = 52;
            this.splitContainer1.TabIndex = 0;
            // 
            // ViewButton
            // 
            this.ViewButton.Location = new System.Drawing.Point(325, 13);
            this.ViewButton.Name = "ViewButton";
            this.ViewButton.Size = new System.Drawing.Size(75, 23);
            this.ViewButton.TabIndex = 2;
            this.ViewButton.Text = "查看详情";
            this.ViewButton.UseVisualStyleBackColor = true;
            // 
            // ExportExcel
            // 
            this.ExportExcel.Location = new System.Drawing.Point(163, 13);
            this.ExportExcel.Name = "ExportExcel";
            this.ExportExcel.Size = new System.Drawing.Size(75, 23);
            this.ExportExcel.TabIndex = 1;
            this.ExportExcel.Text = "生成Excel";
            this.ExportExcel.UseVisualStyleBackColor = true;
            // 
            // TwinkleButton
            // 
            this.TwinkleButton.Location = new System.Drawing.Point(13, 13);
            this.TwinkleButton.Name = "TwinkleButton";
            this.TwinkleButton.Size = new System.Drawing.Size(75, 23);
            this.TwinkleButton.TabIndex = 0;
            this.TwinkleButton.Text = "闪烁";
            this.TwinkleButton.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(441, 209);
            this.dataGridView1.TabIndex = 0;
            // 
            // BusResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 265);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BusResultForm";
            this.Text = "公交搜索结果";
            this.Load += new System.EventHandler(this.BusResultForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ViewButton;
        private System.Windows.Forms.Button ExportExcel;
        private System.Windows.Forms.Button TwinkleButton;
        private System.Windows.Forms.DataGridView dataGridView1;

    }
}