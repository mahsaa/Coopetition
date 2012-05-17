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
            this.outputLog = new System.Windows.Forms.RichTextBox();
            this.btnStrategy = new System.Windows.Forms.Button();
            this.cboStrategies = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // outputLog
            // 
            this.outputLog.Location = new System.Drawing.Point(31, 25);
            this.outputLog.Name = "outputLog";
            this.outputLog.Size = new System.Drawing.Size(737, 382);
            this.outputLog.TabIndex = 1;
            this.outputLog.Text = "";
            // 
            // btnStrategy
            // 
            this.btnStrategy.Location = new System.Drawing.Point(267, 429);
            this.btnStrategy.Name = "btnStrategy";
            this.btnStrategy.Size = new System.Drawing.Size(75, 23);
            this.btnStrategy.TabIndex = 2;
            this.btnStrategy.Text = "Start Run";
            this.btnStrategy.UseVisualStyleBackColor = true;
            this.btnStrategy.Click += new System.EventHandler(this.btnStrategy_Click);
            // 
            // cboStrategies
            // 
            this.cboStrategies.FormattingEnabled = true;
            this.cboStrategies.Items.AddRange(new object[] {
            "Coopetitive",
            "All Competitive",
            "All random"});
            this.cboStrategies.Location = new System.Drawing.Point(128, 429);
            this.cboStrategies.Name = "cboStrategies";
            this.cboStrategies.Size = new System.Drawing.Size(121, 21);
            this.cboStrategies.TabIndex = 3;
            this.cboStrategies.Text = "Coopetitive";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 432);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select a strategy: ";
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 481);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboStrategies);
            this.Controls.Add(this.btnStrategy);
            this.Controls.Add(this.outputLog);
            this.Name = "main";
            this.Text = "main";
            this.Load += new System.EventHandler(this.main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox outputLog;
        private System.Windows.Forms.Button btnStrategy;
        private System.Windows.Forms.ComboBox cboStrategies;
        private System.Windows.Forms.Label label1;
    }
}