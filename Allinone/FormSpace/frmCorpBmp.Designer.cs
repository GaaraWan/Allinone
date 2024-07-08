
namespace Allinone.FormSpace
{
    partial class frmCorpBmp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCorpBmp));
            this.dispUI1 = new JzDisplay.UISpace.DispUI();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSaveExit = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.cboPages = new System.Windows.Forms.ToolStripComboBox();
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
            this.dispUI1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cboPages,
            this.btnSaveExit,
            this.btnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSaveExit
            // 
            this.btnSaveExit.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveExit.Image")));
            this.btnSaveExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveExit.Name = "btnSaveExit";
            this.btnSaveExit.Size = new System.Drawing.Size(88, 22);
            this.btnSaveExit.Text = "保存并退出";
            // 
            // btnExit
            // 
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(52, 22);
            this.btnExit.Text = "退出";
            // 
            // cboPages
            // 
            this.cboPages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPages.Name = "cboPages";
            this.cboPages.Size = new System.Drawing.Size(121, 25);
            // 
            // frmCorpBmp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dispUI1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmCorpBmp";
            this.Text = "定义截图大小的窗口";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JzDisplay.UISpace.DispUI dispUI1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSaveExit;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.ToolStripComboBox cboPages;
    }
}