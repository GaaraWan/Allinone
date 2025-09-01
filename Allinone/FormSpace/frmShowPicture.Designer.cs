namespace Allinone.FormSpace
{
    partial class frmShowPicture
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
            this.dispUI1 = new JzDisplay.UISpace.DispUI();
            this.SuspendLayout();
            // 
            // dispUI1
            // 
            this.dispUI1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dispUI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dispUI1.Location = new System.Drawing.Point(0, 0);
            this.dispUI1.Name = "dispUI1";
            this.dispUI1.Size = new System.Drawing.Size(800, 450);
            this.dispUI1.TabIndex = 1;
            // 
            // frmShowPicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dispUI1);
            this.Name = "frmShowPicture";
            this.Text = "frmShowPicture";
            this.ResumeLayout(false);

        }

        #endregion

        private JzDisplay.UISpace.DispUI dispUI1;
    }
}