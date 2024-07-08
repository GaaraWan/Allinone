namespace Allinone.FormSpace
{
    partial class CPDForm
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
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.cpdUI1 = new Allinone.UISpace.CpdUI();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button6.Location = new System.Drawing.Point(872, 519);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(76, 39);
            this.button6.TabIndex = 8;
            this.button6.Text = "取消";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button4.Location = new System.Drawing.Point(796, 519);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 39);
            this.button4.TabIndex = 7;
            this.button4.Text = "確定";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // cpdUI1
            // 
            this.cpdUI1.Location = new System.Drawing.Point(1, 2);
            this.cpdUI1.Name = "cpdUI1";
            this.cpdUI1.Size = new System.Drawing.Size(952, 514);
            this.cpdUI1.TabIndex = 59;
            // 
            // CPDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 562);
            this.ControlBox = false;
            this.Controls.Add(this.cpdUI1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CPDForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPDForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
        private UISpace.CpdUI cpdUI1;
    }
}