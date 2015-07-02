namespace ChartApp
{
    partial class Main
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sysChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonDisk = new System.Windows.Forms.Button();
            this.buttonMemory = new System.Windows.Forms.Button();
            this.buttonCPU = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
            this.SuspendLayout();
            // 
            // sysChart
            // 
            chartArea4.Name = "ChartArea1";
            this.sysChart.ChartAreas.Add(chartArea4);
            this.sysChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend4.Name = "Legend1";
            this.sysChart.Legends.Add(legend4);
            this.sysChart.Location = new System.Drawing.Point(0, 0);
            this.sysChart.Name = "sysChart";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.sysChart.Series.Add(series4);
            this.sysChart.Size = new System.Drawing.Size(684, 446);
            this.sysChart.TabIndex = 0;
            this.sysChart.Text = "sysChart";
            // 
            // buttonDisk
            // 
            this.buttonDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDisk.Location = new System.Drawing.Point(572, 398);
            this.buttonDisk.Name = "buttonDisk";
            this.buttonDisk.Size = new System.Drawing.Size(100, 36);
            this.buttonDisk.TabIndex = 1;
            this.buttonDisk.Text = "DISK (OFF)";
            this.buttonDisk.UseVisualStyleBackColor = true;
            this.buttonDisk.Click += new System.EventHandler(this.buttonDisk_Click);
            // 
            // buttonMemory
            // 
            this.buttonMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMemory.Location = new System.Drawing.Point(572, 356);
            this.buttonMemory.Name = "buttonMemory";
            this.buttonMemory.Size = new System.Drawing.Size(100, 36);
            this.buttonMemory.TabIndex = 2;
            this.buttonMemory.Text = "MEMORY (OFF)";
            this.buttonMemory.UseVisualStyleBackColor = true;
            this.buttonMemory.Click += new System.EventHandler(this.buttonMemory_Click);
            // 
            // buttonCPU
            // 
            this.buttonCPU.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCPU.Location = new System.Drawing.Point(572, 314);
            this.buttonCPU.Name = "buttonCPU";
            this.buttonCPU.Size = new System.Drawing.Size(100, 36);
            this.buttonCPU.TabIndex = 3;
            this.buttonCPU.Text = "CPU (OFF)";
            this.buttonCPU.UseVisualStyleBackColor = true;
            this.buttonCPU.Click += new System.EventHandler(this.buttonCPU_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 446);
            this.Controls.Add(this.buttonCPU);
            this.Controls.Add(this.buttonMemory);
            this.Controls.Add(this.buttonDisk);
            this.Controls.Add(this.sysChart);
            this.Name = "Main";
            this.Text = "System Metrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart sysChart;
        private System.Windows.Forms.Button buttonDisk;
        private System.Windows.Forms.Button buttonMemory;
        private System.Windows.Forms.Button buttonCPU;
    }
}

