namespace TTCSServer
{
    partial class MainWindows
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindows));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnUserManagenment = new System.Windows.Forms.Button();
            this.BtnScriptManager = new System.Windows.Forms.Button();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.StationStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StationSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StationLastestTimeUpdate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TTCSLogGrid = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SearchStationName = new System.Windows.Forms.ComboBox();
            this.StationEnableSearch = new System.Windows.Forms.CheckBox();
            this.SearchStationEndDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.SearchStationStartDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ClientGrid = new System.Windows.Forms.DataGridView();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MainDevicePanel = new System.Windows.Forms.Panel();
            this.DeviceGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DeviceStatus = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SelectedDevice = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TTCSLogGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClientGrid)).BeginInit();
            this.MainDevicePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DeviceGrid)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnUserManagenment);
            this.groupBox1.Controls.Add(this.BtnScriptManager);
            this.groupBox1.Controls.Add(this.BtnSetup);
            this.groupBox1.Controls.Add(this.StationStatus);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.StationSelection);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1147, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Station Information";
            // 
            // BtnUserManagenment
            // 
            this.BtnUserManagenment.Location = new System.Drawing.Point(516, 28);
            this.BtnUserManagenment.Name = "BtnUserManagenment";
            this.BtnUserManagenment.Size = new System.Drawing.Size(75, 23);
            this.BtnUserManagenment.TabIndex = 5;
            this.BtnUserManagenment.Text = "User";
            this.BtnUserManagenment.UseVisualStyleBackColor = true;
            this.BtnUserManagenment.Click += new System.EventHandler(this.BtnUserManagenment_Click);
            // 
            // BtnScriptManager
            // 
            this.BtnScriptManager.Location = new System.Drawing.Point(435, 28);
            this.BtnScriptManager.Name = "BtnScriptManager";
            this.BtnScriptManager.Size = new System.Drawing.Size(75, 23);
            this.BtnScriptManager.TabIndex = 5;
            this.BtnScriptManager.Text = "Script";
            this.BtnScriptManager.UseVisualStyleBackColor = true;
            this.BtnScriptManager.Click += new System.EventHandler(this.BtnScriptManager_Click);
            // 
            // BtnSetup
            // 
            this.BtnSetup.Location = new System.Drawing.Point(354, 28);
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.Size = new System.Drawing.Size(75, 23);
            this.BtnSetup.TabIndex = 4;
            this.BtnSetup.Text = "Setting";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // StationStatus
            // 
            this.StationStatus.AutoSize = true;
            this.StationStatus.ForeColor = System.Drawing.Color.Red;
            this.StationStatus.Location = new System.Drawing.Point(678, 32);
            this.StationStatus.Name = "StationStatus";
            this.StationStatus.Size = new System.Drawing.Size(37, 13);
            this.StationStatus.TabIndex = 3;
            this.StationStatus.Text = "Offline";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(633, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Status :";
            // 
            // StationSelection
            // 
            this.StationSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StationSelection.FormattingEnabled = true;
            this.StationSelection.Items.AddRange(new object[] {
            "ASTRO SERVER",
            "2.4 Meter Telescope",
            "0.7 Meter Telescope Airfoce",
            "0.7 Meter Telescope Chachoengsao",
            "0.7 Meter Telescope Korat",
            "0.7 Meter Telescope Songkhla",
            "0.7 Meter Telescope Astro Park",
            "0.7 Meter Telescope China",
            "0.7 Meter Telescope USA",
            "0.7 Meter Telescope Austria"});
            this.StationSelection.Location = new System.Drawing.Point(137, 29);
            this.StationSelection.Name = "StationSelection";
            this.StationSelection.Size = new System.Drawing.Size(211, 21);
            this.StationSelection.TabIndex = 1;
            this.StationSelection.SelectedIndexChanged += new System.EventHandler(this.SiteSelection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Station Selection :";
            // 
            // StationLastestTimeUpdate
            // 
            this.StationLastestTimeUpdate.AutoSize = true;
            this.StationLastestTimeUpdate.ForeColor = System.Drawing.Color.Blue;
            this.StationLastestTimeUpdate.Location = new System.Drawing.Point(650, 27);
            this.StationLastestTimeUpdate.Name = "StationLastestTimeUpdate";
            this.StationLastestTimeUpdate.Size = new System.Drawing.Size(10, 13);
            this.StationLastestTimeUpdate.TabIndex = 3;
            this.StationLastestTimeUpdate.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(534, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Lastest data received :";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1147, 5);
            this.panel1.TabIndex = 1;
            // 
            // TTCSLogGrid
            // 
            this.TTCSLogGrid.AllowUserToAddRows = false;
            this.TTCSLogGrid.AllowUserToDeleteRows = false;
            this.TTCSLogGrid.BackgroundColor = System.Drawing.Color.White;
            this.TTCSLogGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.TTCSLogGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TTCSLogGrid.ColumnHeadersVisible = false;
            this.TTCSLogGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column5,
            this.Column4,
            this.Column6});
            this.TTCSLogGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TTCSLogGrid.GridColor = System.Drawing.Color.White;
            this.TTCSLogGrid.Location = new System.Drawing.Point(3, 40);
            this.TTCSLogGrid.Name = "TTCSLogGrid";
            this.TTCSLogGrid.ReadOnly = true;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Blue;
            this.TTCSLogGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.TTCSLogGrid.RowHeadersVisible = false;
            this.TTCSLogGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.TTCSLogGrid.RowTemplate.Height = 17;
            this.TTCSLogGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TTCSLogGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TTCSLogGrid.Size = new System.Drawing.Size(1133, 180);
            this.TTCSLogGrid.TabIndex = 2;
            this.TTCSLogGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.TTCSLogGrid_RowsAdded);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "LogDate";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 150;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "Message";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "LogDataType";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 150;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Value";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 150;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "LogReporter";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 150;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "UserID";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 200;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(5, 69);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.MainDevicePanel);
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(1147, 668);
            this.splitContainer1.SplitterDistance = 249;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1147, 249);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TTCSLogGrid);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1139, 223);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TTCS Logs";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SearchStationName);
            this.panel2.Controls.Add(this.StationEnableSearch);
            this.panel2.Controls.Add(this.SearchStationEndDate);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.SearchStationStartDate);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1133, 37);
            this.panel2.TabIndex = 3;
            // 
            // SearchStationName
            // 
            this.SearchStationName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SearchStationName.Enabled = false;
            this.SearchStationName.FormattingEnabled = true;
            this.SearchStationName.Items.AddRange(new object[] {
            "All Station",
            "TTCS Server",
            "Client Application Interface",
            "Client Web Interface",
            "2.4 Meter Telescope",
            "0.7 Meter Telescope Airfoce",
            "0.7 Meter Telescope Chachoengsao",
            "0.7 Meter Telescope Korat",
            "0.7 Meter Telescope Songkhla",
            "0.7 Meter Telescope China",
            "0.7 Meter Telescope USA",
            "0.7 Meter Telescope Austria"});
            this.SearchStationName.Location = new System.Drawing.Point(89, 8);
            this.SearchStationName.Name = "SearchStationName";
            this.SearchStationName.Size = new System.Drawing.Size(209, 21);
            this.SearchStationName.TabIndex = 3;
            this.SearchStationName.SelectedIndexChanged += new System.EventHandler(this.SearchStationName_SelectedIndexChanged);
            // 
            // StationEnableSearch
            // 
            this.StationEnableSearch.AutoSize = true;
            this.StationEnableSearch.Location = new System.Drawing.Point(864, 11);
            this.StationEnableSearch.Name = "StationEnableSearch";
            this.StationEnableSearch.Size = new System.Drawing.Size(96, 17);
            this.StationEnableSearch.TabIndex = 2;
            this.StationEnableSearch.Text = "Enable Search";
            this.StationEnableSearch.UseVisualStyleBackColor = true;
            this.StationEnableSearch.CheckedChanged += new System.EventHandler(this.AllStationEnableSearch_CheckedChanged);
            // 
            // SearchStationEndDate
            // 
            this.SearchStationEndDate.Enabled = false;
            this.SearchStationEndDate.Location = new System.Drawing.Point(654, 9);
            this.SearchStationEndDate.Name = "SearchStationEndDate";
            this.SearchStationEndDate.Size = new System.Drawing.Size(200, 20);
            this.SearchStationEndDate.TabIndex = 1;
            this.SearchStationEndDate.ValueChanged += new System.EventHandler(this.AllStationStartDate_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(590, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "End Date :";
            // 
            // SearchStationStartDate
            // 
            this.SearchStationStartDate.Enabled = false;
            this.SearchStationStartDate.Location = new System.Drawing.Point(371, 9);
            this.SearchStationStartDate.Name = "SearchStationStartDate";
            this.SearchStationStartDate.Size = new System.Drawing.Size(200, 20);
            this.SearchStationStartDate.TabIndex = 1;
            this.SearchStationStartDate.ValueChanged += new System.EventHandler(this.AllStationStartDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = " Station Name :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Start Date :";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ClientGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1139, 223);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Client Monitoring";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ClientGrid
            // 
            this.ClientGrid.AllowUserToAddRows = false;
            this.ClientGrid.AllowUserToDeleteRows = false;
            this.ClientGrid.BackgroundColor = System.Drawing.Color.White;
            this.ClientGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ClientGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ClientGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12});
            this.ClientGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClientGrid.GridColor = System.Drawing.Color.White;
            this.ClientGrid.Location = new System.Drawing.Point(0, 0);
            this.ClientGrid.Name = "ClientGrid";
            this.ClientGrid.ReadOnly = true;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Blue;
            this.ClientGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.ClientGrid.RowHeadersVisible = false;
            this.ClientGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.ClientGrid.RowTemplate.Height = 17;
            this.ClientGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ClientGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ClientGrid.Size = new System.Drawing.Size(1139, 223);
            this.ClientGrid.TabIndex = 3;
            this.ClientGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ClientGrid_CellMouseDoubleClick);
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Num";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 40;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "IPAddress";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "Port";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "Online Time";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 150;
            // 
            // Column11
            // 
            this.Column11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column11.HeaderText = "Lastest Command";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            // 
            // Column12
            // 
            this.Column12.HeaderText = "Lastest Recive";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.Width = 150;
            // 
            // MainDevicePanel
            // 
            this.MainDevicePanel.Controls.Add(this.DeviceGrid);
            this.MainDevicePanel.Controls.Add(this.panel5);
            this.MainDevicePanel.Controls.Add(this.groupBox2);
            this.MainDevicePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainDevicePanel.Location = new System.Drawing.Point(0, 41);
            this.MainDevicePanel.Name = "MainDevicePanel";
            this.MainDevicePanel.Size = new System.Drawing.Size(1147, 374);
            this.MainDevicePanel.TabIndex = 7;
            // 
            // DeviceGrid
            // 
            this.DeviceGrid.AllowUserToAddRows = false;
            this.DeviceGrid.AllowUserToDeleteRows = false;
            this.DeviceGrid.BackgroundColor = System.Drawing.Color.White;
            this.DeviceGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DeviceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DeviceGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.DeviceGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DeviceGrid.GridColor = System.Drawing.Color.White;
            this.DeviceGrid.Location = new System.Drawing.Point(0, 61);
            this.DeviceGrid.Name = "DeviceGrid";
            this.DeviceGrid.ReadOnly = true;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Blue;
            this.DeviceGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DeviceGrid.RowHeadersVisible = false;
            this.DeviceGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DeviceGrid.RowTemplate.Height = 17;
            this.DeviceGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DeviceGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DeviceGrid.Size = new System.Drawing.Size(1147, 313);
            this.DeviceGrid.TabIndex = 3;
            this.DeviceGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DeviceGrid_CellMouseClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Num";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Command Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Value";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 250;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Update Time";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 56);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1147, 5);
            this.panel5.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DeviceStatus);
            this.groupBox2.Controls.Add(this.StationLastestTimeUpdate);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.SelectedDevice);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1147, 56);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device Selection";
            // 
            // DeviceStatus
            // 
            this.DeviceStatus.AutoSize = true;
            this.DeviceStatus.ForeColor = System.Drawing.Color.Red;
            this.DeviceStatus.Location = new System.Drawing.Point(464, 27);
            this.DeviceStatus.Name = "DeviceStatus";
            this.DeviceStatus.Size = new System.Drawing.Size(37, 13);
            this.DeviceStatus.TabIndex = 5;
            this.DeviceStatus.Text = "Offline";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(419, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Status :";
            // 
            // SelectedDevice
            // 
            this.SelectedDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedDevice.FormattingEnabled = true;
            this.SelectedDevice.Items.AddRange(new object[] {
            "TTCS Server",
            "2.4 Meter Telescope",
            "0.7 Meter Telescope Airfoce",
            "0.7 Meter Telescope Chachoengsao",
            "0.7 Meter Telescope Korat",
            "0.7 Meter Telescope Songkhla",
            "0.7 Meter Telescope Astro Park",
            "0.7 Meter Telescope China",
            "0.7 Meter Telescope USA",
            "0.7 Meter Telescope Austria"});
            this.SelectedDevice.Location = new System.Drawing.Point(137, 24);
            this.SelectedDevice.Name = "SelectedDevice";
            this.SelectedDevice.Size = new System.Drawing.Size(276, 21);
            this.SelectedDevice.TabIndex = 3;
            this.SelectedDevice.SelectedIndexChanged += new System.EventHandler(this.SelectedDevice_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Selected Device :";
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 36);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1147, 5);
            this.panel4.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1147, 36);
            this.panel3.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(507, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Avaliable Device";
            // 
            // MainWindows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 742);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindows";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TTCS Server V 1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindows_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TTCSLogGrid)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ClientGrid)).EndInit();
            this.MainDevicePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DeviceGrid)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label StationLastestTimeUpdate;
        private System.Windows.Forms.Label StationStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox StationSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView TTCSLogGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel MainDevicePanel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BtnSetup;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker SearchStationEndDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker SearchStationStartDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox StationEnableSearch;
        private System.Windows.Forms.ComboBox SearchStationName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridView DeviceGrid;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox SelectedDevice;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Label DeviceStatus;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView ClientGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.Button BtnScriptManager;
        private System.Windows.Forms.Button BtnUserManagenment;
    }
}

