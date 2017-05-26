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
    public partial class UserMonitoring : Form
    {
        private Byte[] DeletePNG = TTCSHelper.ImageToByte(Properties.Resources.delete_19);

        public UserMonitoring()
        {
            InitializeComponent();

            UserType.SelectedIndex = 0;
            GetStationList();
            GetData();

            UserManagement.UserCheckerLoop();
        }

        private void GetStationList()
        {
            List<Object> StationNameStrArr = new List<Object>();
            StationNameStrArr.Add("All Station");

            List<STATIONNAME> AllStationName = Enum.GetValues(typeof(STATIONNAME)).Cast<STATIONNAME>().ToList();
            AllStationName.Remove(AllStationName.FirstOrDefault(Item => Item == STATIONNAME.NULL));
            foreach (STATIONNAME ThisStationName in AllStationName)
                StationNameStrArr.Add(ThisStationName.ToString());

            UserStationPermission.Items.AddRange(StationNameStrArr.ToArray());
            UserStationPermission.SelectedIndex = 0;
        }

        private void GetData()
        {
            List<UserTB> UserList = UserManagement.GetAllUser();

            if (UserType.Text != "All Type")
                UserList = UserList.Where(Item => Item.UserPermissionType != null ? Item.UserPermissionType.Contains(UserType.Text) : false).ToList();

            if (UserStationPermission.Text != "All Station")
                UserList = UserList.Where(Item => Item.UserStationPermission != null ? Item.UserStationPermission.Contains(UserStationPermission.Text) : false).ToList();

            UserDataGrid.Rows.Clear();
            foreach (UserTB ThisUser in UserList)
            {
                List<Object> GridValue = new List<Object>();
                GridValue.Add(DeletePNG);
                GridValue.Add(ThisUser.UserID);
                GridValue.Add(ThisUser.UserName);
                GridValue.Add(ThisUser.UserLoginName);
                GridValue.Add(ThisUser.UserPermissionType);

                if (ThisUser.UserStationPermission != null)
                    GridValue.Add(ThisUser.UserStationPermission.Replace(",", ", "));

                UserDataGrid.Rows.Add(GridValue.ToArray());
            }
        }

        //---------------------------------------------------------------------------------------Event Handler--------------------------------------------------------------------------------------------

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void UserMonitoring_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void UserDataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                String UserID = UserDataGrid[1, e.RowIndex].Value.ToString();
                if (e.ColumnIndex == 0)
                {
                    if (MessageBox.Show("Do you want to delete this user?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        UserManagement.DeleteUser(UserID);
                        UserDataGrid.Rows.RemoveAt(e.RowIndex);


                    }
                }
                else
                {
                    UserModification ObjUserModification = new UserModification(UserID);
                    ObjUserModification.ShowDialog(this);

                    UserTB ThisUser = UserManagement.GetUserByID(UserID);
                    UserDataGrid[2, e.RowIndex].Value = ThisUser.UserName;
                    UserDataGrid[3, e.RowIndex].Value = ThisUser.UserLoginName;
                    UserDataGrid[4, e.RowIndex].Value = ThisUser.UserPermissionType;
                    UserDataGrid[5, e.RowIndex].Value = ThisUser.UserStationPermission;
                }
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            UserModification ObjUserModification = new UserModification(null);
            ObjUserModification.ShowDialog(this);

            GetData();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            GetData();
        }
    }
}