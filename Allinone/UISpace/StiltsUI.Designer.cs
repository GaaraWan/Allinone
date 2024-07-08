namespace Allinone.UISpace
{
    partial class StiltsUI
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
            this.dispUI1 = new JzDisplay.UISpace.DispUI();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dispUI1
            // 
            this.dispUI1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dispUI1.Location = new System.Drawing.Point(2, 3);
            this.dispUI1.Name = "dispUI1";
            this.dispUI1.Size = new System.Drawing.Size(458, 355);
            this.dispUI1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(325, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 44);
            this.label1.TabIndex = 2;
            this.label1.Text = "PASS\r\n123\r\n544";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // StiltsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GreenYellow;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dispUI1);
            this.Name = "StiltsUI";
            this.Size = new System.Drawing.Size(463, 361);
            this.ResumeLayout(false);

        }

        #endregion

        private JzDisplay.UISpace.DispUI dispUI1;
        private System.Windows.Forms.Label label1;
    }
}
