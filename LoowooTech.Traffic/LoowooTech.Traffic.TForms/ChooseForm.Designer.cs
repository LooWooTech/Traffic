namespace LoowooTech.Traffic.TForms
{
    partial class ChooseForm
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.Number1 = new System.Windows.Forms.Label();
            this.OneFeature = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.End1 = new System.Windows.Forms.Label();
            this.Start1 = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.Number2 = new System.Windows.Forms.Label();
            this.TwoFeature = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.End2 = new System.Windows.Forms.Label();
            this.Start2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(565, 262);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.splitContainer2.Panel1.Controls.Add(this.Number1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2.Controls.Add(this.OneFeature);
            this.splitContainer2.Panel2.Controls.Add(this.label9);
            this.splitContainer2.Panel2.Controls.Add(this.End1);
            this.splitContainer2.Panel2.Controls.Add(this.Start1);
            this.splitContainer2.Size = new System.Drawing.Size(565, 125);
            this.splitContainer2.SplitterDistance = 157;
            this.splitContainer2.TabIndex = 0;
            // 
            // Number1
            // 
            this.Number1.AutoSize = true;
            this.Number1.Font = new System.Drawing.Font("微软雅黑", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Number1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Number1.Location = new System.Drawing.Point(12, 26);
            this.Number1.Name = "Number1";
            this.Number1.Size = new System.Drawing.Size(92, 52);
            this.Number1.TabIndex = 1;
            this.Number1.Text = "316";
            // 
            // OneFeature
            // 
            this.OneFeature.Location = new System.Drawing.Point(317, 90);
            this.OneFeature.Name = "OneFeature";
            this.OneFeature.Size = new System.Drawing.Size(75, 23);
            this.OneFeature.TabIndex = 3;
            this.OneFeature.Text = "查看详情";
            this.OneFeature.UseVisualStyleBackColor = true;
            this.OneFeature.Click += new System.EventHandler(this.OneFeature_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Arrow_32x32;
            this.label9.Location = new System.Drawing.Point(170, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 27);
            this.label9.TabIndex = 2;
            this.label9.Text = " ";
            // 
            // End1
            // 
            this.End1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.End1.Location = new System.Drawing.Point(202, 33);
            this.End1.Name = "End1";
            this.End1.Size = new System.Drawing.Size(190, 20);
            this.End1.TabIndex = 1;
            this.End1.Text = "终点站";
            this.End1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Start1
            // 
            this.Start1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Start1.Location = new System.Drawing.Point(3, 33);
            this.Start1.Name = "Start1";
            this.Start1.Size = new System.Drawing.Size(170, 20);
            this.Start1.TabIndex = 0;
            this.Start1.Text = "起始站";
            this.Start1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.splitContainer3.Panel1.Controls.Add(this.Number2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer3.Panel2.Controls.Add(this.TwoFeature);
            this.splitContainer3.Panel2.Controls.Add(this.label10);
            this.splitContainer3.Panel2.Controls.Add(this.End2);
            this.splitContainer3.Panel2.Controls.Add(this.Start2);
            this.splitContainer3.Size = new System.Drawing.Size(565, 133);
            this.splitContainer3.SplitterDistance = 158;
            this.splitContainer3.TabIndex = 0;
            // 
            // Number2
            // 
            this.Number2.AutoSize = true;
            this.Number2.Font = new System.Drawing.Font("微软雅黑", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Number2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Number2.Location = new System.Drawing.Point(12, 29);
            this.Number2.Name = "Number2";
            this.Number2.Size = new System.Drawing.Size(92, 52);
            this.Number2.TabIndex = 2;
            this.Number2.Text = "316";
            // 
            // TwoFeature
            // 
            this.TwoFeature.Location = new System.Drawing.Point(316, 98);
            this.TwoFeature.Name = "TwoFeature";
            this.TwoFeature.Size = new System.Drawing.Size(75, 23);
            this.TwoFeature.TabIndex = 4;
            this.TwoFeature.Text = "查看详情";
            this.TwoFeature.UseVisualStyleBackColor = true;
            this.TwoFeature.Click += new System.EventHandler(this.TwoFeature_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Image = global::LoowooTech.Traffic.TForms.Properties.Resources.Arrow_32x32;
            this.label10.Location = new System.Drawing.Point(169, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 27);
            this.label10.TabIndex = 3;
            this.label10.Text = " ";
            // 
            // End2
            // 
            this.End2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.End2.Location = new System.Drawing.Point(205, 29);
            this.End2.Name = "End2";
            this.End2.Size = new System.Drawing.Size(186, 20);
            this.End2.TabIndex = 2;
            this.End2.Text = "终点站";
            this.End2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Start2
            // 
            this.Start2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Start2.Location = new System.Drawing.Point(6, 29);
            this.Start2.Name = "Start2";
            this.Start2.Size = new System.Drawing.Size(166, 20);
            this.Start2.TabIndex = 2;
            this.Start2.Text = "起始站";
            this.Start2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChooseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 262);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ChooseForm";
            this.Text = "路线选择";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ChooseForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label Number1;
        private System.Windows.Forms.Label End1;
        private System.Windows.Forms.Label Start1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label Number2;
        private System.Windows.Forms.Label End2;
        private System.Windows.Forms.Label Start2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button OneFeature;
        private System.Windows.Forms.Button TwoFeature;
        private System.Windows.Forms.Label label9;
    }
}