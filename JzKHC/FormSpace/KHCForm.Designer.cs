namespace JzKHC.FormSpace
{
    partial class KHCForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.recipedbuiControl1 = new JzKHC.DBSpace.RECIPEDBUIControl();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button2.Location = new System.Drawing.Point(145, 651);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(67, 48);
            this.button2.TabIndex = 227;
            this.button2.Text = "刪除";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.button3.Location = new System.Drawing.Point(898, 651);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(67, 48);
            this.button3.TabIndex = 232;
            this.button3.Text = "離開";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // recipedbuiControl1
            // 
            this.recipedbuiControl1.DBStatus = JzKHC.DBSpace.DBStatusEnum.ADD;
            this.recipedbuiControl1.Location = new System.Drawing.Point(2, 112);
            this.recipedbuiControl1.Name = "recipedbuiControl1";
            this.recipedbuiControl1.Size = new System.Drawing.Size(1131, 595);
            this.recipedbuiControl1.TabIndex = 233;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 31);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(225, 20);
            this.comboBox1.TabIndex = 234;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 235;
            this.label1.Text = "參數名稱";
            // 
            // KHCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1136, 710);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.recipedbuiControl1);
            this.Name = "KHCForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "鍵高參數設定視窗";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private DBSpace.RECIPEDBUIControl recipedbuiControl1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
    }
}