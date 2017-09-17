using DataKeeper;
using DataKeeper.Engine;
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
    public partial class ScriptMonitoring : Form
    {
        private Byte[] DeletePNG = TTCSHelper.ImageToByte(Properties.Resources.delete_19);

        public ScriptMonitoring()
        {
            InitializeComponent();
            GetStatioName();
            ScriptManager.ScriptLifeTimeValue = Properties.Settings.Default.ScriptLifeTimeValue;
        }

        private void GetStatioName()
        {
            List<STATIONNAME> AllStationName = Enum.GetValues(typeof(STATIONNAME)).Cast<STATIONNAME>().ToList();
            foreach (STATIONNAME StationName in AllStationName)
                StationSelection.Items.Add(StationName.ToString());

            StationSelection.SelectedIndex = 0;
        }

        public void GetScript()
        {
            STATIONNAME? StationName = TTCSHelper.StationConveter(StationSelection.Text);

            if (StationName != null)
                ScriptEngine.RefreshScript(StationName.Value);
        }

        public void AddScript(List<ScriptStructureNew> NewScriptCollection)
        {
            ScriptGrid.Invoke((Action)(() =>
            {
                ScriptGrid.Rows.Clear();

                foreach (ScriptStructureNew ThisScript in NewScriptCollection)
                    ScriptGrid.Rows.Add(DeletePNG, ThisScript.ScriptID, ThisScript.BlockID, ThisScript.Life, ThisScript.DeviceName, ThisScript.CommandName, String.Join(", ", ThisScript.Parameters),
                        ThisScript.ScritpState, new DateTime(Convert.ToInt64(ThisScript.ExecuteionTimeStart)).ToString(), new DateTime(Convert.ToInt64(ThisScript.ExecuteionTimeEnd)));
            }));
        }

        public void AddScriptMessage(String Message)
        {
            try
            {
                ScriptMessage.Invoke((Action)(() =>
                {
                    if (ScriptMessage.RowCount > 0)
                    {
                        int LastestRow = ScriptMessage.RowCount - 1;

                        if (ScriptMessage[0, LastestRow].Value.ToString() != Message)
                            ScriptMessage.Rows.Add(Message, DateTime.Now.ToString());
                        else
                            ScriptMessage[1, LastestRow].Value = DateTime.Now.ToString();
                    }
                    else
                        ScriptMessage.Rows.Add(Message, DateTime.Now.ToString());
                }));
            }
            catch (Exception e)
            {
            }
        }

        //----------------------------------------------------------------------------------Event Handler-------------------------------------------------------------------------------------

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ScriptMonitoring_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            GetScript();
        }

        private void BtnSendToStation_Click(object sender, EventArgs e)
        {

        }
    }
}
