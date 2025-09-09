namespace Allinone.FormSpace.PADForm
{
    partial class PadExtendForm
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.padeX1UI1 = new Allinone.FormSpace.PADForm.PADExtend.PADEX1UI();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.padeX1UI1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 48);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(851, 453);
            this.panel3.TabIndex = 18;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button6);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 501);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(851, 52);
            this.panel2.TabIndex = 17;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button6.Location = new System.Drawing.Point(772, 4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(76, 45);
            this.button6.TabIndex = 10;
            this.button6.Text = "取消";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button4.Location = new System.Drawing.Point(696, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 45);
            this.button4.TabIndex = 9;
            this.button4.Text = "確定";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(851, 48);
            this.panel1.TabIndex = 16;
            // 
            // padeX1UI1
            // 
            this.padeX1UI1.Location = new System.Drawing.Point(39, 35);
            this.padeX1UI1.Name = "padeX1UI1";
            this.padeX1UI1.Size = new System.Drawing.Size(636, 435);
            this.padeX1UI1.TabIndex = 0;
            // 
            // PadExtendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 553);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "PadExtendForm";
            this.Text = "PadExtendForm";
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel1;
        private PADExtend.PADEX1UI padeX1UI1;
    }
}