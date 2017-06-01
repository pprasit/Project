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
            AddScriptToGrid();
            SetDataGridViewOrder();

            ScriptLifeTime.Text = Properties.Settings.Default.ScriptLifeTimeValue.ToString();
            ScriptManager.ScriptLifeTimeValue = Properties.Settings.Default.ScriptLifeTimeValue;
        }

        private void SetDataGridViewOrder()
        {
            this.ScriptGrid.Sort(this.ScriptGrid.Columns[6], ListSortDirection.Ascending);
        }

        public void RemoveScriptFromGrid(ScriptTB ThisScript)
        {
            ThreadRemoveDataGridRow(ThisScript.BlockID, ThisScript.ExecutionNumber);
        }

        public void AddSingleScriptToGrid(ScriptTB ThisScript)
        {
            Object[] ObjectValue = new Object[] {
                DeletePNG,
                ThisScript.BlockID,
                ThisScript.BlockName,
                ThisScript.ExecutionNumber,
                ThisScript.CommandCounter,
                ThisScript.StationName,
                ThisScript.ExecutionTimeStart.Value.ToLocalTime(),
                ThisScript.ExecutionTimeEnd.Value.ToLocalTime(),
                ThisScript.DeviceName,
                ThisScript.DeviceCategory,
                ThisScript.CommandName,
                ThisScript.Owner,
                ThisScript.Parameter,
                ThisScript.ScriptState,
                false
            };

            ThreadAddDataGridRow(ObjectValue);
        }

        public void AddScriptToGrid()
        {
            List<ScriptTB> AllScript = ScriptManager.GetScriptList();
            AllScript = AllScript.OrderBy(Item => Item.ExecutionTimeStart).ThenBy(Item => Item.BlockID).ThenBy(Item => Item.ExecutionNumber).ToList();

            ThreadClearDataGridRow();
            foreach (ScriptTB ThisScript in AllScript)
            {
                Object[] ObjectValue = new Object[] {
                DeletePNG,
                ThisScript.BlockID,
                ThisScript.BlockName,
                ThisScript.ExecutionNumber,
                ThisScript.CommandCounter,
                ThisScript.StationName,
                ThisScript.ExecutionTimeStart.Value.ToLocalTime(),
                ThisScript.ExecutionTimeEnd.Value.ToLocalTime(),
                ThisScript.DeviceName,
                ThisScript.DeviceCategory,
                ThisScript.CommandName,
                ThisScript.Owner,
                ThisScript.Parameter,
                ThisScript.ScriptState,
                false
            };

                ThreadAddDataGridRow(ObjectValue);
            }
        }

        public void UpdateScriptGrid(ScriptTB ThisScript)
        {
            ThreadModifyDataGridRow(ThisScript);
        }

        //------------------------------------------------------------Thread Remove Row DataGridView Handler----------------------------------------------------------------

        private void ThreadRemoveDataGridRow(String BlockID, int ExecutionNumber)
        {
            if (ScriptGrid.InvokeRequired)
                ScriptGrid.Invoke(new MethodInvoker(delegate { ActionRemoveDataGridRow(BlockID, ExecutionNumber); }));
            else
                ActionRemoveDataGridRow(BlockID, ExecutionNumber);
        }

        private void ActionRemoveDataGridRow(String BlockID, int ExecutionNumber)
        {
            for (int i = 0; i < ScriptGrid.RowCount; i++)
                if (ScriptGrid[1, i].Value.ToString() == BlockID && ScriptGrid[3, i].Value.ToString() == ExecutionNumber.ToString())
                {
                    ScriptGrid.Rows.RemoveAt(i);
                    break;
                }
        }

        //------------------------------------------------------------Thread Modify Row DataGridView Handler----------------------------------------------------------------

        private void ThreadModifyDataGridRow(ScriptTB ThisScript)
        {
            if (ScriptGrid.InvokeRequired)
                ScriptGrid.Invoke(new MethodInvoker(delegate { ActionModifyDataGridRow(ThisScript); }));
            else
                ActionModifyDataGridRow(ThisScript);
        }

        private void ActionModifyDataGridRow(ScriptTB ThisScript)
        {
            for (int i = 0; i < ScriptGrid.RowCount; i++)
                if (ScriptGrid[1, i].Value.ToString() == ThisScript.BlockID && Convert.ToInt32(ScriptGrid[3, i].Value) == ThisScript.ExecutionNumber)
                {
                    ScriptGrid[2, i].Value = ThisScript.BlockName;
                    ScriptGrid[4, i].Value = ThisScript.CommandCounter;
                    ScriptGrid[5, i].Value = ThisScript.StationName;
                    ScriptGrid[6, i].Value = ThisScript.ExecutionTimeStart.Value.ToLocalTime();
                    ScriptGrid[7, i].Value = ThisScript.ExecutionTimeEnd.Value.ToLocalTime();
                    ScriptGrid[8, i].Value = ThisScript.DeviceName;
                    ScriptGrid[9, i].Value = ThisScript.DeviceCategory;
                    ScriptGrid[10, i].Value = ThisScript.CommandName;
                    ScriptGrid[11, i].Value = ThisScript.Owner;
                    ScriptGrid[12, i].Value = ThisScript.Parameter;
                    ScriptGrid[13, i].Value = ThisScript.ScriptState;
                    break;
                }
        }

        //------------------------------------------------------------Thread Add Row DataGridView Handler----------------------------------------------------------------

        private void ThreadClearDataGridRow()
        {
            if (ScriptGrid.InvokeRequired)
                ScriptGrid.Invoke(new MethodInvoker(delegate { ActionClearDataGridRow(); }));
            else
                ActionClearDataGridRow();
        }

        private void ActionClearDataGridRow()
        {
            ScriptGrid.Rows.Clear();
        }

        //------------------------------------------------------------Thread Add Row DataGridView Handler----------------------------------------------------------------

        private void ThreadAddDataGridRow(Object[] DataGridRow)
        {
            if (ScriptGrid.InvokeRequired)
                ScriptGrid.Invoke(new MethodInvoker(delegate { ActionAddDataGridRow(DataGridRow); }));
            else
                ActionAddDataGridRow(DataGridRow);
        }

        private void ActionAddDataGridRow(Object[] DataGridRow)
        {
            ScriptGrid.Rows.Add(DataGridRow);
        }

        //----------------------------------------------------------------------------Event Handler------------------------------------------------------------------

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void ScriptMonitoring_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            ScriptManager.LoadScriptFromDB();
            AddScriptToGrid();
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to remove all script from database?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ScriptManager.RemoveAllScriptDB();
                AddScriptToGrid();
            }
        }

        private void ScriptGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    if (MessageBox.Show("Do you want to delete selected script>", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        String BlockID = ScriptGrid[1, e.RowIndex].Value.ToString();
                        int ExecutionNumber = Convert.ToInt32(ScriptGrid[3, e.RowIndex].Value);

                        if (!ScriptManager.RemoveScript(BlockID, ExecutionNumber))
                            MessageBox.Show("Can not delete selected script from database.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        AddScriptToGrid();
                    }
                }
            }
        }

        private void ScriptGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (e.ColumnIndex == 14)
                {
                    String SelectedBlockID = ScriptGrid[1, e.RowIndex].Value.ToString();

                    Boolean ColumnValue = Convert.ToBoolean(ScriptGrid[14, e.RowIndex].Value);
                    ScriptGrid[14, e.RowIndex].Value = !ColumnValue;

                    for (int i = 0; i < ScriptGrid.RowCount; i++)
                        if (ScriptGrid[1, i].Value.ToString() == SelectedBlockID)
                            ScriptGrid[14, i].Value = !ColumnValue;
                }
            }
        }

        private void BtnSendToStation_Click(object sender, EventArgs e)
        {
            Boolean IsCheck = false;

            for (int i = 0; i < ScriptGrid.RowCount; i++)
            {
                if (Convert.ToBoolean(ScriptGrid[14, i].Value))
                    IsCheck = true;
            }

            if (!IsCheck)
            {
                MessageBox.Show("Please select the script to send to client.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


        }

        private void ScriptLifeTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.ScriptLifeTimeValue = Convert.ToDouble(ScriptLifeTime.Text);
            Properties.Settings.Default.ScriptLifeTimeValue = ScriptManager.ScriptLifeTimeValue;
            Properties.Settings.Default.Save();
        }
    }
}
