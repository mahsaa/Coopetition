namespace Coopetition
{
    partial class main
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
            this.btnStartRun = new System.Windows.Forms.Button();
            this.outputLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnStartRun
            // 
            this.btnStartRun.Location = new System.Drawing.Point(31, 212);
            this.btnStartRun.Name = "btnStartRun";
            this.btnStartRun.Size = new System.Drawing.Size(75, 23);
            this.btnStartRun.TabIndex = 0;
            this.btnStartRun.Text = "Start Run";
            this.btnStartRun.UseVisualStyleBackColor = true;
            this.btnStartRun.Click += new System.EventHandler(this.btnStartRun_Click);
            // 
            // outputLog
            // 
            this.outputLog.Location = new System.Drawing.Point(31, 25);
            this.outputLog.Name = "outputLog";
            this.outputLog.Size = new System.Drawing.Size(382, 165);
            this.outputLog.TabIndex = 1;
            this.outputLog.Text = "";
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 271);
            this.Controls.Add(this.outputLog);
            this.Controls.Add(this.btnStartRun);
            this.Name = "main";
            this.Text = "main";
            this.Load += new System.EventHandler(this.main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartRun;
        private System.Windows.Forms.RichTextBox outputLog;
    }
}