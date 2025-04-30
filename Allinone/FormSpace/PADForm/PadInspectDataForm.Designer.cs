namespace Allinone.FormSpace.PADForm
{
    partial class PadInspectDataForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.padUI1 = new Allinone.UISpace.MSRUISpace.PadUI();
            this.ipdV1UI1 = new Allinone.UISpace.MSRUISpace.IPDV1UI();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "检测方式";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(9, 25);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(173, 20);
            this.comboBox1.TabIndex = 2;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button6.Location = new System.Drawing.Point(744, 574);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(76, 45);
            this.button6.TabIndex = 10;
            this.button6.Text = "取消";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button4.Location = new System.Drawing.Point(668, 574);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 45);
            this.button4.TabIndex = 9;
            this.button4.Text = "確定";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // padUI1
            // 
            this.padUI1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.padUI1.Location = new System.Drawing.Point(9, 51);
            this.padUI1.Name = "padUI1";
            this.padUI1.Size = new System.Drawing.Size(811, 517);
            this.padUI1.TabIndex = 11;
            this.padUI1.Visible = false;
            // 
            // ipdV1UI1
            // 
            this.ipdV1UI1.Location = new System.Drawing.Point(200, 25);
            this.ipdV1UI1.Name = "ipdV1UI1";
            this.ipdV1UI1.Size = new System.Drawing.Size(811, 517);
            this.ipdV1UI1.TabIndex = 12;
            this.ipdV1UI1.Visible = false;
            // 
            // PadInspectDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 622);
            this.Controls.Add(this.ipdV1UI1);
            this.Controls.Add(this.padUI1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Name = "PadInspectDataForm";
            this.Text = "PadInspectDataForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
        private UISpace.MSRUISpace.PadUI padUI1;
        private UISpace.MSRUISpace.IPDV1UI ipdV1UI1;
    }
}