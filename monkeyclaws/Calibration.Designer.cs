namespace monkeyclaws
{
    partial class Calibration
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Aldhabi", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(58, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(513, 75);
            this.label1.TabIndex = 0;
            this.label1.Text = "Place your hands on a horizontal plane";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.progressBar1.Location = new System.Drawing.Point(-12, 33);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(699, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Value = 50;
            // 
            // Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 396);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Name = "Calibration";
            this.Text = "Calibration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseCalibration);
            this.Load += new System.EventHandler(this.Calibration_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}