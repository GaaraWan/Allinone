namespace JzASN.UISpace
{
    partial class AsnUI
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
            this.components = new System.ComponentModel.Container();
            this.dispUI1 = new JzDisplay.UISpace.DispUI();
            this.label4 = new System.Windows.Forms.Label();
            this.myPropertyGrid1 = new JetEazy.BasicSpace.myPropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataTreeListViewNew = new BrightIdeasSoftware.DataTreeListView();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTreeListViewNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // dispUI1
            // 
            this.dispUI1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dispUI1.Location = new System.Drawing.Point(3, 2);
            this.dispUI1.Name = "dispUI1";
            this.dispUI1.Size = new System.Drawing.Size(732, 478);
            this.dispUI1.TabIndex = 48;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(3, 483);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(525, 28);
            this.label4.TabIndex = 47;
            this.label4.Text = "F7:新增，F8:刪除";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // myPropertyGrid1
            // 
            this.myPropertyGrid1.CategorySplitterColor = System.Drawing.Color.Green;
            this.myPropertyGrid1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.myPropertyGrid1.Location = new System.Drawing.Point(737, 306);
            this.myPropertyGrid1.Name = "myPropertyGrid1";
            this.myPropertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.myPropertyGrid1.Size = new System.Drawing.Size(209, 205);
            this.myPropertyGrid1.TabIndex = 50;
            this.myPropertyGrid1.ToolbarVisible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(737, 291);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 49;
            this.label1.Text = "設定";
            // 
            // dataGridView2
            // 
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(770, 134);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView2.Size = new System.Drawing.Size(0, 35);
            this.dataGridView2.TabIndex = 51;
            // 
            // dataTreeListViewNew
            // 
            this.dataTreeListViewNew.CellEditUseWholeCell = false;
            this.dataTreeListViewNew.DataSource = null;
            this.dataTreeListViewNew.GridLines = true;
            this.dataTreeListViewNew.HideSelection = false;
            this.dataTreeListViewNew.Location = new System.Drawing.Point(737, 19);
            this.dataTreeListViewNew.Name = "dataTreeListViewNew";
            this.dataTreeListViewNew.RootKeyValueString = "";
            this.dataTreeListViewNew.ShowGroups = false;
            this.dataTreeListViewNew.ShowKeyColumns = false;
            this.dataTreeListViewNew.Size = new System.Drawing.Size(209, 269);
            this.dataTreeListViewNew.TabIndex = 52;
            this.dataTreeListViewNew.UseCompatibleStateImageBehavior = false;
            this.dataTreeListViewNew.View = System.Windows.Forms.View.Details;
            this.dataTreeListViewNew.VirtualMode = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(737, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 53;
            this.label2.Text = "明細";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button2.Location = new System.Drawing.Point(646, 483);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 27);
            this.button2.TabIndex = 54;
            this.button2.Text = "取得圖片";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.numericUpDown2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(594, 486);
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(47, 22);
            this.numericUpDown2.TabIndex = 56;
            this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown2.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(534, 489);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 16);
            this.label11.TabIndex = 55;
            this.label11.Text = "位移大小";
            // 
            // AsnUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataTreeListViewNew);
            this.Controls.Add(this.myPropertyGrid1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dispUI1);
            this.Controls.Add(this.label4);
            this.Name = "AsnUI";
            this.Size = new System.Drawing.Size(952, 514);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTreeListViewNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JzDisplay.UISpace.DispUI dispUI1;
        private System.Windows.Forms.Label label4;
        private JetEazy.BasicSpace.myPropertyGrid myPropertyGrid1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private BrightIdeasSoftware.DataTreeListView dataTreeListViewNew;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label11;
    }
}
