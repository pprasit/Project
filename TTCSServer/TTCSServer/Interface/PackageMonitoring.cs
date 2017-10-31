using System;
using System.Collections.Concurrent;
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
    public partial class PackageMonitoring : Form
    {
        struct ReturningMonitoringInfo
        {
            public String CommandName { get; set; }
            public String ByteCounter { get; set; }
            public String DataMessage { get; set; }
            public String ConditionCase { get; set; }
        }

        private String IPAddress = null;
        private String Port = null;
        private DateTime? StartTime = null;
        private String ActiveCommandName = null;

        public PackageMonitoring()
        {
            InitializeComponent();
            SetDataGridStyle();
        }

        private void SetDataGridStyle()
        {
            LastestCommandGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;

            Padding newPadding = new Padding(0, 1, 0, 20);
            this.LastestCommandGrid.RowTemplate.DefaultCellStyle.Padding = newPadding;
        }

        public void OnlineTimeSetter()
        {
            if (StartTime == null)
                return;

            TimeSpan Span = DateTime.UtcNow - StartTime.Value;
            AddCommand(OnlineTime, Span.ToString(@"dd\.hh\:mm\:ss"));
        }

        public void ClientInformationSetter(String IPAddress, String Port)
        {
            this.IPAddress = IPAddress;
            this.Port = Port;

            AddCommand(IPaddressText, IPAddress);
            AddCommand(PortText, Port.ToString());
        }

        public void StartTimeSetter(DateTime StartTime)
        {
            this.StartTime = StartTime;
        }

        public void ReciveCommandSetter(String[] LastestCommand, String LastestRecive)
        {
            String LastestCommandStr = String.Join(Environment.NewLine, LastestCommand);
            AddCommand(this.LastestCommandGrid, LastestCommandStr);
            AddCommand(this.LastestRecive, LastestRecive);
        }

        public void SetInformation(List<String> Messages)
        {
            ClearGrid(LastestCommandGrid);

            foreach (String Message in Messages)
            {
                AddCommand(LastestCommandGrid, Message);
                AddCommand(LastestRecive, Message);
            }
        }

        public void SetNewInformation(String Message)
        {
            AddCommand(LastestCommandGrid, Message);
            AddCommand(LastestRecive, Message);
        }

        public void RemoveReturningInfo(String IPAddress, String Port)
        {
            if (this.IPAddress == IPAddress && this.Port == Port)
            {
                //ReturningInfo.Clear();
                SetComboboxClear(ReturningCommand);
                AddCommand(IPaddressText, "");
                AddCommand(PortText, "");
                AddCommand(OnlineTime, "");
                AddCommand(LastestCommandGrid, "");
                AddCommand(LastestRecive, "");
                AddCommand(ByteCounter, "");
                AddCommand(DataMessage, "");
            }
        }

        public void ReturningMessageSetter(String TotalPackageByte, String CommandName, String DataMessage, String ConditionCase)
        {
            Boolean IsFound = false;
            SetComboboxItem(ReturningCommand, CommandName);

            if (CommandName == ActiveCommandName)
            {
                AddCommand(ByteCounter, String.Format("{0:#,#}", Convert.ToInt64(TotalPackageByte)) + " Byte, Condition : " + ConditionCase);
                AddCommand(this.DataMessage, DataMessage);
            }
        }

        public Boolean ClientChecker(String IPAddress, String Port)
        {
            if (this.IPAddress == IPAddress && this.Port == Port)
                return true;

            return false;
        }

        //--------------------------------------------------------------------------------------Thread Clear Combobox------------------------------------------------------------------------------------

        private void SetComboboxClear(ComboBox ControlCtr)
        {
            if (ControlCtr.InvokeRequired)
            {
                ControlCtr.Invoke((Action)(() => ActionComboboxClear(ControlCtr)));
                return;
            }

            ActionComboboxClear(ControlCtr);
        }

        private void ActionComboboxClear(ComboBox ControlCtr)
        {
            ControlCtr.Items.Clear();
        }

        //--------------------------------------------------------------------------------------Thread Add Combobox------------------------------------------------------------------------------------

        private void SetComboboxItem(ComboBox ControlCtr, String CommandName)
        {
            if (ControlCtr.InvokeRequired)
            {
                ControlCtr.Invoke((Action)(() => ActionComboboxItem(ControlCtr, CommandName)));
                return;
            }

            ActionComboboxItem(ControlCtr, CommandName);
        }

        private void ActionComboboxItem(ComboBox ControlCtr, String CommandName)
        {
            try
            {
                for (int i = 0; i < ControlCtr.Items.Count; i++)
                {
                    if (CommandName == null || CommandName == ControlCtr.Items[i].ToString())
                        return;
                }

                ControlCtr.Items.Add(CommandName);
            }
            catch { }
        }

        //--------------------------------------------------------------------------------------Thread Textbox------------------------------------------------------------------------------------

        private static void AddCommand(Control ControlCtr, String Message)
        {
            if (ControlCtr.InvokeRequired)
            {
                ControlCtr.Invoke((Action)(() => ActionText(ControlCtr, Message)));
                return;
            }

            ActionText(ControlCtr, Message);
        }

        private static void ActionText(Control ControlCtr, String Message)
        {
            try
            {
                if (ControlCtr.Name == "LastestCommand")
                {
                    DataGridView Grid = (DataGridView)ControlCtr;
                    Grid.Rows.Add(Grid.RowCount + 1, Message);
                }
                else
                    ControlCtr.Text = Message;
            }
            catch { }
        }

        //--------------------------------------------------------------------------------------Thread ClearGrid------------------------------------------------------------------------------------

        private static void ClearGrid(DataGridView ControlCtr)
        {
            if (ControlCtr.InvokeRequired)
            {
                ControlCtr.Invoke((Action)(() => ActionClearGrid(ControlCtr)));
                return;
            }

            ActionClearGrid(ControlCtr);
        }

        private static void ActionClearGrid(DataGridView ControlCtr)
        {
            ControlCtr.Rows.Clear();
        }

        //--------------------------------------------------------------------------------------Event Handler------------------------------------------------------------------------------------

        private void PackageMonitoring_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ReturningCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveCommandName = ReturningCommand.Text;

            //foreach (KeyValuePair<String, ReturningMonitoringInfo> ThisInfo in ReturningInfo)
            //{
            //    if (ThisInfo.Key == ActiveCommandName)
            //    {
            //        ReturningMonitoringInfo DataNode = ThisInfo.Value;
            //        ByteCounter.Text = String.Format("{0:#,#}", Convert.ToInt64(DataNode.ByteCounter)) + " Byte, Condition : " + DataNode.ConditionCase;
            //        DataMessage.Text = DataNode.DataMessage;
            //    }
            //}

        }

        private void LastestCommand_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView ThidGrid = (DataGridView)sender;

            if (ThidGrid.Rows.Count > 500)
                ThidGrid.Rows.RemoveAt(0);

            ThidGrid.CurrentCell = ThidGrid.Rows[ThidGrid.RowCount - 1].Cells[0];
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            LastestCommandGrid.Rows.Clear();
        }

        private void LastestCommand_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                AddCommand(this.RecivingText, LastestCommandGrid[1, e.RowIndex].Value.ToString());
        }
    }
}