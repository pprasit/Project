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
    public partial class UserModification : Form
    {
        private String UserID = null;

        public UserModification(String UserID)
        {
            InitializeComponent();

            this.UserID = UserID;

            GetPermissionType();
            GetStationPermission();
            GetUserInformation();
        }

        private void GetUserInformation()
        {
            if (UserID != null)
            {
                UserTB ThisUser = UserManagement.GetUserByID(UserID);

                if (ThisUser != null)
                {
                    UserName.Text = ThisUser.UserName;
                    UserLoginName.Text = ThisUser.UserLoginName;
                    UserLoginPassword.Text = ThisUser.UserLoginPassword;

                    String PermissionStr = ThisUser.UserPermissionType;
                    if (PermissionStr != null)
                    {
                        String[] PermissionArr = PermissionStr.Split(new char[] { ',' });
                        for (int i = 0; i < UserPermissionType.RowCount; i++)
                            UserPermissionType[0, i].Value = false;

                        foreach (String ThisPermission in PermissionArr)
                            for (int i = 0; i < UserPermissionType.RowCount; i++)
                                if (UserPermissionType[1, i].Value.ToString() == ThisPermission)
                                {
                                    UserPermissionType[0, i].Value = true;
                                    break;
                                }
                    }

                    String StationStr = ThisUser.UserStationPermission;
                    if (StationStr != null)
                    {
                        String[] StationArr = StationStr.Split(new char[] { ',' });
                        for (int i = 0; i < UserStationPermission.RowCount; i++)
                            UserStationPermission[0, i].Value = false;

                        foreach (String ThisStation in StationArr)
                            for (int i = 0; i < UserStationPermission.RowCount; i++)
                                if (UserStationPermission[1, i].Value.ToString() == ThisStation)
                                {
                                    UserStationPermission[0, i].Value = true;
                                    break;
                                }
                    }
                }
            }
        }

        private void GetPermissionType()
        {
            UserPermissionType.Rows.Add(false, "All Type");
            UserPermissionType.Rows.Add(false, "Administrator");
            UserPermissionType.Rows.Add(false, "General");
            UserPermissionType.Rows.Add(true, "Guest");
        }

        private void GetStationPermission()
        {
            List<STATIONNAME> AllStationName = Enum.GetValues(typeof(STATIONNAME)).Cast<STATIONNAME>().ToList();
            UserStationPermission.Rows.Add(false, "All Station");

            foreach (STATIONNAME ThisStation in AllStationName)
            {
                if (ThisStation != STATIONNAME.NULL)
                    UserStationPermission.Rows.Add(false, ThisStation.ToString());
            }
        }

        private Boolean VerifyData()
        {
            if (UserName.Text == "")
            {
                MessageBox.Show("Invalid user name. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (UserManagement.IsUserNameExsiting(UserID, UserName.Text))
            {
                MessageBox.Show("Invalid user name. Your user name is already exist in the system.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (UserLoginName.Text == "")
            {
                MessageBox.Show("Invalid user login name. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (UserLoginPassword.Text == "")
            {
                MessageBox.Show("Invalid user password. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (UserLoginPassword.Text != ConfirmPassword.Text)
            {
                MessageBox.Show("Your password is not the same with confirm password. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (UserManagement.IsLoginInformationExisting(UserID, UserLoginName.Text, UserLoginPassword.Text))
            {
                MessageBox.Show("Invalid user login name and password. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            Boolean ISUserPermissionTypeCheck = false;
            for (int i = 0; i < UserPermissionType.RowCount; i++)
                if (Convert.ToBoolean(UserPermissionType[0, i].Value))
                    ISUserPermissionTypeCheck = true;

            if (!ISUserPermissionTypeCheck)
            {
                MessageBox.Show("Please choose the permission for this user.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            Boolean ISStationAccessCheck = false;
            for (int i = 0; i < UserStationPermission.RowCount; i++)
                if (Convert.ToBoolean(UserStationPermission[0, i].Value))
                    ISStationAccessCheck = true;

            if (!ISStationAccessCheck)
            {
                MessageBox.Show("Please choose the station access for this user.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        //------------------------------------------------------------------------------Event Handler-------------------------------------------------------

        private void UserPermissionType_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                UserPermissionType[0, e.RowIndex].Value = !Convert.ToBoolean(UserPermissionType[0, e.RowIndex].Value);
        }

        private void UserStationPermission_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                UserStationPermission[0, e.RowIndex].Value = !Convert.ToBoolean(UserStationPermission[0, e.RowIndex].Value);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (VerifyData())
            {
                List<String> PermissionArray = new List<String>();
                for (int i = 0; i < UserPermissionType.RowCount; i++)
                    if (Convert.ToBoolean(UserPermissionType[0, i].Value))
                        PermissionArray.Add(UserPermissionType[1, i].Value.ToString());

                List<String> StationArray = new List<String>();
                for (int i = 0; i < UserStationPermission.RowCount; i++)
                    if (Convert.ToBoolean(UserStationPermission[0, i].Value))
                        StationArray.Add(UserStationPermission[1, i].Value.ToString());

                if (UserID == null)
                {
                    UserID = TTCSHelper.GenNewID();
                    UserManagement.AddUser(UserID, UserName.Text, UserLoginName.Text, UserLoginPassword.Text, String.Join(",", PermissionArray), String.Join(",", StationArray));
                }
                else
                    UserManagement.UpdateUser(UserID, UserName.Text, UserLoginName.Text, UserLoginPassword.Text, String.Join(",", PermissionArray), String.Join(",", StationArray));

                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
