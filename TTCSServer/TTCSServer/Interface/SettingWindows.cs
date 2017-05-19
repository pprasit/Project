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
            TTCSDevicelostConnection.Value = Properties.Settings.Default.DeviceLostConnectionTime;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
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
