namespace Allinone.FormSpace
{
    partial class MeasureDataForm
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.mbUI1 = new Allinone.UISpace.MSRUISpace.MbUI();
            this.bkUI1 = new Allinone.UISpace.MSRUISpace.BkUI();
            this.colorUI1 = new Allinone.UISpace.MSRUISpace.ColorUI();
            this.solderUI1 = new Allinone.UISpace.MSRUISpace.SolderUI();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 32);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(173, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "量測方式";
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button6.Location = new System.Drawing.Point(543, 361);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(76, 45);
            this.button6.TabIndex = 8;
            this.button6.Text = "取消";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button4.Location = new System.Drawing.Point(467, 361);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 45);
            this.button4.TabIndex = 7;
            this.button4.Text = "確定";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // mbUI1
            // 
            this.mbUI1.Location = new System.Drawing.Point(270, 19);
            this.mbUI1.Name = "mbUI1";
            this.mbUI1.Size = new System.Drawing.Size(607, 298);
            this.mbUI1.TabIndex = 11;
            // 
            // bkUI1
            // 
            this.bkUI1.Location = new System.Drawing.Point(166, 119);
            this.bkUI1.Name = "bkUI1";
            this.bkUI1.Size = new System.Drawing.Size(607, 298);
            this.bkUI1.TabIndex = 10;
            // 
            // colorUI1
            // 
            this.colorUI1.Location = new System.Drawing.Point(72, 151);
            this.colorUI1.Name = "colorUI1";
            this.colorUI1.Size = new System.Drawing.Size(607, 298);
            this.colorUI1.TabIndex = 9;
            // 
            // solderUI1
            // 
            this.solderUI1.Location = new System.Drawing.Point(343, 68);
            this.solderUI1.Name = "solderUI1";
            this.solderUI1.Size = new System.Drawing.Size(607, 298);
            this.solderUI1.TabIndex = 12;
            // 
            // MeasureDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 407);
            this.ControlBox = false;
            this.Controls.Add(this.solderUI1);
            this.Controls.Add(this.mbUI1);
            this.Controls.Add(this.bkUI1);
            this.Controls.Add(this.colorUI1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Name = "MeasureDataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MeasureDataForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
        private UISpace.MSRUISpace.ColorUI colorUI1;
        private UISpace.MSRUISpace.BkUI bkUI1;
        private UISpace.MSRUISpace.MbUI mbUI1;
        private UISpace.MSRUISpace.SolderUI solderUI1;
    }
}