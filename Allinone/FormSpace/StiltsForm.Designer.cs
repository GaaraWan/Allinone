namespace Allinone.FormSpace
{
    partial class StiltsForm
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
            this.stiltsUI1 = new Allinone.UISpace.StiltsUI();
            this.SuspendLayout();
            // 
            // stiltsUI1
            // 
            this.stiltsUI1.BackColor = System.Drawing.Color.GreenYellow;
            this.stiltsUI1.Location = new System.Drawing.Point(2, 2);
            this.stiltsUI1.Name = "stiltsUI1";
            this.stiltsUI1.Size = new System.Drawing.Size(463, 361);
            this.stiltsUI1.TabIndex = 0;
            // 
            // StiltsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 364);
            this.Controls.Add(this.stiltsUI1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StiltsForm";
            this.Text = "StiltsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private UISpace.StiltsUI stiltsUI1;
    }
}