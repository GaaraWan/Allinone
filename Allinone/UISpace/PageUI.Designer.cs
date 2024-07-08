namespace Allinone.UISpace
{
    partial class PageUI
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.dispUI1 = new JzDisplay.UISpace.DispUI();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.subUI1 = new Allinone.UISpace.SubUI();
            this.atreeUI1 = new Allinone.UISpace.AtreeUI();
            this.ainfoUI1 = new Allinone.UISpace.AinfoUI();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(4, 734);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(421, 28);
            this.label4.TabIndex = 45;
            this.label4.Text = "F6:新增分支，F7:新增同層，F8:刪除，F9:複製，F10:新增框體，F11:刪除框體";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(762, 738);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 16);
            this.checkBox1.TabIndex = 47;
            this.checkBox1.Text = "Lock";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(864, 735);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 21);
            this.numericUpDown1.TabIndex = 48;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(917, 738);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 16);
            this.label11.TabIndex = 49;
            this.label11.Text = "位移大小";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.numericUpDown2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(977, 735);
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(47, 21);
            this.numericUpDown2.TabIndex = 50;
            this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown2.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // dispUI1
            // 
            this.dispUI1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dispUI1.Location = new System.Drawing.Point(4, 3);
            this.dispUI1.Name = "dispUI1";
            this.dispUI1.Size = new System.Drawing.Size(1020, 729);
            this.dispUI1.TabIndex = 46;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(815, 738);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(48, 16);
            this.checkBox2.TabIndex = 51;
            this.checkBox2.Text = "Only";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(709, 738);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(60, 16);
            this.checkBox3.TabIndex = 52;
            this.checkBox3.Text = "Detail";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // subUI1
            // 
            this.subUI1.BackColor = System.Drawing.Color.GreenYellow;
            this.subUI1.Location = new System.Drawing.Point(812, 466);
            this.subUI1.Name = "subUI1";
            this.subUI1.Size = new System.Drawing.Size(212, 269);
            this.subUI1.TabIndex = 53;
            // 
            // atreeUI1
            // 
            this.atreeUI1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.atreeUI1.Location = new System.Drawing.Point(1030, 3);
            this.atreeUI1.Name = "atreeUI1";
            this.atreeUI1.Size = new System.Drawing.Size(387, 323);
            this.atreeUI1.TabIndex = 44;
            // 
            // ainfoUI1
            // 
            this.ainfoUI1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ainfoUI1.Location = new System.Drawing.Point(1030, 332);
            this.ainfoUI1.Name = "ainfoUI1";
            this.ainfoUI1.Size = new System.Drawing.Size(387, 428);
            this.ainfoUI1.TabIndex = 43;
            // 
            // PageUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.subUI1);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.atreeUI1);
            this.Controls.Add(this.dispUI1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ainfoUI1);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Name = "PageUI";
            this.Size = new System.Drawing.Size(1420, 765);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AinfoUI ainfoUI1;
        private AtreeUI atreeUI1;
        private System.Windows.Forms.Label label4;
        private JzDisplay.UISpace.DispUI dispUI1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private SubUI subUI1;
    }
}
