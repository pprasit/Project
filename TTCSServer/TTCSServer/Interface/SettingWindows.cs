using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTCSServer.Interface
{
    public partial class SettingWindows : Form
    {
        public SettingWindows()
        {
            InitializeComponent();
            LoadSetting();
        }

        private void LoadSetting()
        {
            DatabaseName.Text = Properties.Settings.Default.DatabaseName;
            DatabaseUserName.Text = Properties.Settings.Default.DatabaseUserName;
            DatabasePassword.Text = Properties.Settings.Default.DatabasePassword;
            DatabaseServerName.Text = Properties.Settings.Default.DatabaseServerName;
            TTCSDevicelostConnection.Value = Properties.Settings.Default.DeviceLostConnectionTime;
            SocketServerAddress.Text = Properties.Settings.Default.SocketServerAddress;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DatabaseName = DatabaseName.Text;
            Properties.Settings.Default.DatabaseUserName = DatabaseUserName.Text;
            Properties.Settings.Default.DatabasePassword = DatabasePassword.Text;
            Properties.Settings.Default.DatabaseServerName = DatabaseServerName.Text;

            Properties.Settings.Default.SocketServerAddress = SocketServerAddress.Text;
            Properties.Settings.Default.DeviceLostConnectionTime = Convert.ToInt32(TTCSDevicelostConnection.Value);
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
