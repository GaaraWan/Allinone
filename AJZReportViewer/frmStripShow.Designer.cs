﻿namespace AJZReportViewer
{
    partial class frmStripShow
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
            this.DS = new JzDisplay.UISpace.DispUI();
            this.SuspendLayout();
            // 
            // DS
            // 
            this.DS.Cursor = System.Windows.Forms.Cursors.Default;
            this.DS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DS.Location = new System.Drawing.Point(0, 0);
            this.DS.Name = "DS";
            this.DS.Size = new System.Drawing.Size(800, 450);
            this.DS.TabIndex = 0;
            // 
            // frmStripShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DS);
            this.Name = "frmStripShow";
            this.Text = "frmStripShow";
            this.ResumeLayout(false);

        }

        #endregion

        private JzDisplay.UISpace.DispUI DS;
    }
}