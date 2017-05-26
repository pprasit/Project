namespace TTCSServer.Interface
{
    partial class UserModification
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
            this.UserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.UserLoginName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.UserLoginPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.UserStationPermission = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.UserPermissionType = new System.Windows.Forms.DataGridView();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.Column3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.ConfirmPassword = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.UserStationPermission)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserPermissionType)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name :";
            // 
            // UserName
            // 
            this.UserName.Location = new System.Drawing.Point(123, 25);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(218, 20);
            this.UserName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Login Name :";
            // 
            // UserLoginName
            // 
            this.UserLoginName.Location = new System.Drawing.Point(123, 51);
            this.UserLoginName.Name = "UserLoginName";
            this.UserLoginName.Size = new System.Drawing.Size(218, 20);
            this.UserLoginName.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Login Password :";
            // 
            // UserLoginPassword
            // 
            this.UserLoginPassword.Location = new System.Drawing.Point(123, 77);
            this.UserLoginPassword.Name = "UserLoginPassword";
            this.UserLoginPassword.PasswordChar = '*';
            this.UserLoginPassword.Size = new System.Drawing.Size(218, 20);
            this.UserLoginPassword.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 238);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Station Access :";
            // 
            // UserStationPermission
            // 
            this.UserStationPermission.AllowUserToAddRows = false;
            this.UserStationPermission.AllowUserToDeleteRows = false;
            this.UserStationPermission.AllowUserToResizeColumns = false;
            this.UserStationPermission.AllowUserToResizeRows = false;
            this.UserStationPermission.BackgroundColor = System.Drawing.Color.White;
            this.UserStationPermission.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserStationPermission.ColumnHeadersVisible = false;
            this.UserStationPermission.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.UserStationPermission.Location = new System.Drawing.Point(123, 238);
            this.UserStationPermission.Name = "UserStationPermission";
            this.UserStationPermission.ReadOnly = true;
            this.UserStationPermission.RowHeadersVisible = false;
            this.UserStationPermission.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UserStationPermission.Size = new System.Drawing.Size(218, 103);
            this.UserStationPermission.TabIndex = 6;
            this.UserStationPermission.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.UserStationPermission_CellMouseClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 129);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Permission :";
            // 
            // UserPermissionType
            // 
            this.UserPermissionType.AllowUserToAddRows = false;
            this.UserPermissionType.AllowUserToDeleteRows = false;
            this.UserPermissionType.AllowUserToResizeColumns = false;
            this.UserPermissionType.AllowUserToResizeRows = false;
            this.UserPermissionType.BackgroundColor = System.Drawing.Color.White;
            this.UserPermissionType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserPermissionType.ColumnHeadersVisible = false;
            this.UserPermissionType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.dataGridViewTextBoxColumn1});
            this.UserPermissionType.Location = new System.Drawing.Point(123, 129);
            this.UserPermissionType.Name = "UserPermissionType";
            this.UserPermissionType.ReadOnly = true;
            this.UserPermissionType.RowHeadersVisible = false;
            this.UserPermissionType.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UserPermissionType.Size = new System.Drawing.Size(218, 103);
            this.UserPermissionType.TabIndex = 5;
            this.UserPermissionType.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.UserPermissionType_CellMouseClick);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(123, 347);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(106, 39);
            this.BtnSave.TabIndex = 7;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(235, 347);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(106, 39);
            this.BtnCancel.TabIndex = 8;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // Column3
            // 
            this.Column3.HeaderText = "";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 30;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "StationName";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 30;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "StationName";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Confirm Password :";
            // 
            // ConfirmPassword
            // 
            this.ConfirmPassword.Location = new System.Drawing.Point(123, 103);
            this.ConfirmPassword.Name = "ConfirmPassword";
            this.ConfirmPassword.PasswordChar = '*';
            this.ConfirmPassword.Size = new System.Drawing.Size(218, 20);
            this.ConfirmPassword.TabIndex = 4;
            // 
            // UserModification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 403);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.UserPermissionType);
            this.Controls.Add(this.UserStationPermission);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ConfirmPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.UserLoginPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.UserLoginName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserModification";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Modification";
            ((System.ComponentModel.ISupportInitialize)(this.UserStationPermission)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserPermissionType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox UserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox UserLoginName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UserLoginPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView UserStationPermission;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView UserPermissionType;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ConfirmPassword;
    }
}