namespace Allinone.FormSpace
{
    partial class frmTestCommon
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTestCommon));
            this.dispUI1 = new JzDisplay.UISpace.DispUI();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnTest = new System.Windows.Forms.ToolStripButton();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.lblResult = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dispUI1
            // 
            this.dispUI1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dispUI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dispUI1.Location = new System.Drawing.Point(0, 25);
            this.dispUI1.Name = "dispUI1";
            this.dispUI1.Size = new System.Drawing.Size(800, 425);
            this.dispUI1.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.btnTest,
            this.lblResult});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnTest
            // 
            this.btnTest.Image = ((System.Drawing.Image)(resources.GetObject("btnTest.Image")));
            this.btnTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(76, 22);
            this.btnTest.Text = "测试按钮";
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(76, 22);
            this.btnLoad.Text = "载入图片";
            // 
            // lblResult
            // 
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(17, 22);
            this.lblResult.Text = "...";
            // 
            // frmTestCommon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dispUI1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmTestCommon";
            this.Text = "frmTestCommon";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JzDisplay.UISpace.DispUI dispUI1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnTest;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripLabel lblResult;
    }
}