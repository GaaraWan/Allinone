namespace JetEazy.UISpace
{
    partial class RptUI
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button16 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button16
            // 
            this.button16.BackColor = System.Drawing.Color.Red;
            this.button16.Location = new System.Drawing.Point(75, 33);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(66, 22);
            this.button16.TabIndex = 283;
            this.button16.Text = "結束記錄";
            this.button16.UseVisualStyleBackColor = false;
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button13.Location = new System.Drawing.Point(145, 32);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(66, 22);
            this.button13.TabIndex = 282;
            this.button13.Text = "匯出資料";
            this.button13.UseVisualStyleBackColor = false;
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button12.Location = new System.Drawing.Point(3, 4);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(66, 22);
            this.button12.TabIndex = 281;
            this.button12.Text = "開始記錄";
            this.button12.UseVisualStyleBackColor = false;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Location = new System.Drawing.Point(75, 4);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(136, 23);
            this.label15.TabIndex = 280;
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RptUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.label15);
            this.Name = "RptUI";
            this.Size = new System.Drawing.Size(213, 58);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Label label15;
    }
}
