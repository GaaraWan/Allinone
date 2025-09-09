namespace Allinone.FormSpace.PADForm.PADExtend
{
    partial class PADEX1UI
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pg = new System.Windows.Forms.PropertyGrid();
            this.DS1 = new JzDisplay.UISpace.DispUI();
            this.SuspendLayout();
            // 
            // pg
            // 
            this.pg.Dock = System.Windows.Forms.DockStyle.Right;
            this.pg.Location = new System.Drawing.Point(367, 0);
            this.pg.Name = "pg";
            this.pg.Size = new System.Drawing.Size(269, 435);
            this.pg.TabIndex = 0;
            // 
            // DS1
            // 
            this.DS1.Cursor = System.Windows.Forms.Cursors.Default;
            this.DS1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DS1.Location = new System.Drawing.Point(0, 0);
            this.DS1.Name = "DS1";
            this.DS1.Size = new System.Drawing.Size(367, 435);
            this.DS1.TabIndex = 8;
            // 
            // PADEX1UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DS1);
            this.Controls.Add(this.pg);
            this.Name = "PADEX1UI";
            this.Size = new System.Drawing.Size(636, 435);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pg;
        private JzDisplay.UISpace.DispUI DS1;
    }
}
