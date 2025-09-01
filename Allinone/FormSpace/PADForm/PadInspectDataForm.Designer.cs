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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.padG1UI1 = new Allinone.FormSpace.PADForm.PadInspect.PADG1UI();
            this.ipdV1UI1 = new Allinone.UISpace.MSRUISpace.IPDV1UI();
            this.padUI1 = new Allinone.UISpace.MSRUISpace.PadUI();
            this.padG2UI1 = new Allinone.FormSpace.PADForm.PadInspect.PADG2UI();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "检测方式";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(173, 20);
            this.comboBox1.TabIndex = 2;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button6.Location = new System.Drawing.Point(745, 4);
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
            this.button4.Location = new System.Drawing.Point(669, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 45);
            this.button4.TabIndex = 9;
            this.button4.Text = "確定";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(824, 48);
            this.panel1.TabIndex = 13;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button6);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 570);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(824, 52);
            this.panel2.TabIndex = 14;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.padG2UI1);
            this.panel3.Controls.Add(this.padG1UI1);
            this.panel3.Controls.Add(this.ipdV1UI1);
            this.panel3.Controls.Add(this.padUI1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 48);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(824, 522);
            this.panel3.TabIndex = 15;
            // 
            // padG1UI1
            // 
            this.padG1UI1.Location = new System.Drawing.Point(236, 149);
            this.padG1UI1.Name = "padG1UI1";
            this.padG1UI1.Size = new System.Drawing.Size(700, 500);
            this.padG1UI1.TabIndex = 13;
            this.padG1UI1.Visible = false;
            // 
            // ipdV1UI1
            // 
            this.ipdV1UI1.Location = new System.Drawing.Point(144, 35);
            this.ipdV1UI1.Name = "ipdV1UI1";
            this.ipdV1UI1.Size = new System.Drawing.Size(811, 517);
            this.ipdV1UI1.TabIndex = 12;
            this.ipdV1UI1.Visible = false;
            // 
            // padUI1
            // 
            this.padUI1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.padUI1.Location = new System.Drawing.Point(68, 20);
            this.padUI1.Name = "padUI1";
            this.padUI1.Size = new System.Drawing.Size(811, 517);
            this.padUI1.TabIndex = 11;
            this.padUI1.Visible = false;
            // 
            // padG2UI1
            // 
            this.padG2UI1.Location = new System.Drawing.Point(319, 76);
            this.padG2UI1.Name = "padG2UI1";
            this.padG2UI1.Size = new System.Drawing.Size(583, 403);
            this.padG2UI1.TabIndex = 14;
            // 
            // PadInspectDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 622);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "PadInspectDataForm";
            this.Text = "PadInspectDataForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
        private UISpace.MSRUISpace.PadUI padUI1;
        private UISpace.MSRUISpace.IPDV1UI ipdV1UI1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private PadInspect.PADG1UI padG1UI1;
        private PadInspect.PADG2UI padG2UI1;
    }
}