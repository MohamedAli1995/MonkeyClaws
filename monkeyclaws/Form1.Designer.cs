namespace monkeyclaws
{
    partial class Form1
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
            this.Calibrate = new System.Windows.Forms.Button();
            this.Configure = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Calibrate
            // 
            this.Calibrate.Location = new System.Drawing.Point(256, 87);
            this.Calibrate.Name = "Calibrate";
            this.Calibrate.Size = new System.Drawing.Size(196, 42);
            this.Calibrate.TabIndex = 2;
            this.Calibrate.Text = "Calibrate";
            this.Calibrate.UseVisualStyleBackColor = true;
            this.Calibrate.Click += new System.EventHandler(this.Calibrate_Click);
            // 
            // Configure
            // 
            this.Configure.Location = new System.Drawing.Point(256, 159);
            this.Configure.Name = "Configure";
            this.Configure.Size = new System.Drawing.Size(196, 42);
            this.Configure.TabIndex = 3;
            this.Configure.Text = "Configure";
            this.Configure.UseVisualStyleBackColor = true;
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(256, 230);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(196, 42);
            this.Start.TabIndex = 4;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(132, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 37);
            this.label1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 408);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Configure);
            this.Controls.Add(this.Calibrate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Calibrate;
        private System.Windows.Forms.Button Configure;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Label label1;
    }
}

