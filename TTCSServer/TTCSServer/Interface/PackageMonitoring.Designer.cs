namespace TTCSServer.Interface
{
    partial class PackageMonitoring
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnClear = new System.Windows.Forms.Button();
            this.LastestCommandGrid = new System.Windows.Forms.DataGridView();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnKick = new System.Windows.Forms.Button();
            this.BtnPause = new System.Windows.Forms.Button();
            this.LastestRecive = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.OnlineTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PortText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IPaddressText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DataMessage = new System.Windows.Forms.TextBox();
            this.ReturningCommand = new System.Windows.Forms.ComboBox();
            this.ByteCounter = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.RecivingText = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LastestCommandGrid)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnClear);
            this.groupBox1.Controls.Add(this.LastestCommandGrid);
            this.groupBox1.Controls.Add(this.BtnClose);
            this.groupBox1.Controls.Add(this.BtnKick);
            this.groupBox1.Controls.Add(this.BtnPause);
            this.groupBox1.Controls.Add(this.LastestRecive);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.OnlineTime);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.PortText);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.IPaddressText);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(958, 265);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Client Information";
            // 
            // BtnClear
            // 
            this.BtnClear.Location = new System.Drawing.Point(381, 232);
            this.BtnClear.Name = "BtnClear";
            this.BtnClear.Size = new System.Drawing.Size(75, 23);
            this.BtnClear.TabIndex = 4;
            this.BtnClear.Text = "Clear";
            this.BtnClear.UseVisualStyleBackColor = true;
            this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // LastestCommandGrid
            // 
            this.LastestCommandGrid.AllowUserToAddRows = false;
            this.LastestCommandGrid.AllowUserToDeleteRows = false;
            this.LastestCommandGrid.BackgroundColor = System.Drawing.Color.White;
            this.LastestCommandGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LastestCommandGrid.ColumnHeadersVisible = false;
            this.LastestCommandGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column1});
            this.LastestCommandGrid.Location = new System.Drawing.Point(122, 56);
            this.LastestCommandGrid.Name = "LastestCommandGrid";
            this.LastestCommandGrid.ReadOnly = true;
            this.LastestCommandGrid.RowHeadersVisible = false;
            this.LastestCommandGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.LastestCommandGrid.Size = new System.Drawing.Size(830, 144);
            this.LastestCommandGrid.TabIndex = 3;
            this.LastestCommandGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.LastestCommand_CellMouseClick);
            this.LastestCommandGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.LastestCommand_RowsAdded);
            // 
            // Column2
            // 
            this.Column2.HeaderText = "";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 50;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "Message";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(284, 232);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 2;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            // 
            // BtnKick
            // 
            this.BtnKick.Location = new System.Drawing.Point(203, 232);
            this.BtnKick.Name = "BtnKick";
            this.BtnKick.Size = new System.Drawing.Size(75, 23);
            this.BtnKick.TabIndex = 2;
            this.BtnKick.Text = "Kick";
            this.BtnKick.UseVisualStyleBackColor = true;
            // 
            // BtnPause
            // 
            this.BtnPause.Location = new System.Drawing.Point(122, 232);
            this.BtnPause.Name = "BtnPause";
            this.BtnPause.Size = new System.Drawing.Size(75, 23);
            this.BtnPause.TabIndex = 2;
            this.BtnPause.Text = "Pause";
            this.BtnPause.UseVisualStyleBackColor = true;
            // 
            // LastestRecive
            // 
            this.LastestRecive.BackColor = System.Drawing.Color.White;
            this.LastestRecive.Location = new System.Drawing.Point(122, 206);
            this.LastestRecive.Name = "LastestRecive";
            this.LastestRecive.ReadOnly = true;
            this.LastestRecive.Size = new System.Drawing.Size(830, 20);
            this.LastestRecive.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 209);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Lastest Recive :";
            // 
            // OnlineTime
            // 
            this.OnlineTime.BackColor = System.Drawing.Color.White;
            this.OnlineTime.Location = new System.Drawing.Point(474, 30);
            this.OnlineTime.Name = "OnlineTime";
            this.OnlineTime.ReadOnly = true;
            this.OnlineTime.Size = new System.Drawing.Size(100, 20);
            this.OnlineTime.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(399, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Online Time :";
            // 
            // PortText
            // 
            this.PortText.BackColor = System.Drawing.Color.White;
            this.PortText.Location = new System.Drawing.Point(281, 30);
            this.PortText.Name = "PortText";
            this.PortText.ReadOnly = true;
            this.PortText.Size = new System.Drawing.Size(100, 20);
            this.PortText.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(243, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Lastest Command :";
            // 
            // IPaddressText
            // 
            this.IPaddressText.BackColor = System.Drawing.Color.White;
            this.IPaddressText.Location = new System.Drawing.Point(122, 30);
            this.IPaddressText.Name = "IPaddressText";
            this.IPaddressText.ReadOnly = true;
            this.IPaddressText.Size = new System.Drawing.Size(100, 20);
            this.IPaddressText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IPAddress :";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 270);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(958, 5);
            this.panel1.TabIndex = 1;
            // 
            // DataMessage
            // 
            this.DataMessage.BackColor = System.Drawing.Color.White;
            this.DataMessage.Location = new System.Drawing.Point(131, 67);
            this.DataMessage.Multiline = true;
            this.DataMessage.Name = "DataMessage";
            this.DataMessage.ReadOnly = true;
            this.DataMessage.Size = new System.Drawing.Size(830, 175);
            this.DataMessage.TabIndex = 3;
            // 
            // ReturningCommand
            // 
            this.ReturningCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReturningCommand.FormattingEnabled = true;
            this.ReturningCommand.Location = new System.Drawing.Point(131, 14);
            this.ReturningCommand.Name = "ReturningCommand";
            this.ReturningCommand.Size = new System.Drawing.Size(830, 21);
            this.ReturningCommand.TabIndex = 2;
            this.ReturningCommand.SelectedIndexChanged += new System.EventHandler(this.ReturningCommand_SelectedIndexChanged);
            // 
            // ByteCounter
            // 
            this.ByteCounter.BackColor = System.Drawing.Color.White;
            this.ByteCounter.Location = new System.Drawing.Point(131, 41);
            this.ByteCounter.Name = "ByteCounter";
            this.ByteCounter.ReadOnly = true;
            this.ByteCounter.Size = new System.Drawing.Size(830, 20);
            this.ByteCounter.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(43, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Data Message :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(51, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Byte Counter :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Returning Command :";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(5, 275);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(958, 262);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.RecivingText);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(950, 236);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Reciving";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // RecivingText
            // 
            this.RecivingText.BackColor = System.Drawing.Color.White;
            this.RecivingText.Location = new System.Drawing.Point(102, 15);
            this.RecivingText.Multiline = true;
            this.RecivingText.Name = "RecivingText";
            this.RecivingText.ReadOnly = true;
            this.RecivingText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.RecivingText.Size = new System.Drawing.Size(842, 215);
            this.RecivingText.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Data Message :";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.DataMessage);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.ReturningCommand);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.ByteCounter);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(950, 236);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sending";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // PackageMonitoring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 542);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackageMonitoring";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PackageMonitoring";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PackageMonitoring_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LastestCommandGrid)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PortText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPaddressText;
        private System.Windows.Forms.TextBox OnlineTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnKick;
        private System.Windows.Forms.Button BtnPause;
        private System.Windows.Forms.TextBox LastestRecive;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox DataMessage;
        private System.Windows.Forms.ComboBox ReturningCommand;
        private System.Windows.Forms.TextBox ByteCounter;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView LastestCommandGrid;
        private System.Windows.Forms.Button BtnClear;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox RecivingText;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage tabPage2;
    }
}