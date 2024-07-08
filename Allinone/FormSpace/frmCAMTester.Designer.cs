namespace Allinone.FormSpace
{
    partial class frmCAMTester
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
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dispUI1
            // 
            this.dispUI1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dispUI1.Location = new System.Drawing.Point(12, 12);
            this.dispUI1.Name = "dispUI1";
            this.dispUI1.Size = new System.Drawing.Size(581, 419);
            this.dispUI1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(599, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 49);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmCAMTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dispUI1);
            this.Name = "frmCAMTester";
            this.Text = "frmCAMTester";
            this.ResumeLayout(false);

        }

        #endregion

        private JzDisplay.UISpace.DispUI dispUI1;
        private System.Windows.Forms.Button button1;
    }
}