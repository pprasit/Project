using DataKeeper.Engine;
using DataKeeper.Engine.Command;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataKeeper
{
    public static class UIHandler
    {
        private static STATIONNAME ActiveStation;
        private static String ActiveDeviceNameStr;
        private static DataGridView DeviceGrid = null;
        private static ComboBox DeviceCombo = null;
        private static Label DeviceStatus = null;
        private static Label StationStatus = null;
        private static Label StationLastestTimeUpdate = null;
        private static List<String> OnlineDevice = null;

        public static void SetActiveStation(STATIONNAME ActiveStation, DataGridView DeviceGrid, ComboBox DeviceCombo, Label DeviceStatus, Label StationStatus, Label StationLastestTimeUpdate)
        {
            UIHandler.ActiveStation = ActiveStation;
            UIHandler.DeviceGrid = DeviceGrid;
            UIHandler.DeviceCombo = DeviceCombo;
            UIHandler.DeviceStatus = DeviceStatus;
            UIHandler.StationStatus = StationStatus;
            UIHandler.StationLastestTimeUpdate = StationLastestTimeUpdate;

            DeviceGrid.Rows.Clear();
            DeviceCombo.Items.Clear();
            SetDeviceList();
        }

        public static void SetDeviceList()
        {
            if (AstroData.KeeperData == null)
                return;

            StationHandler ThisStation = AstroData.KeeperData.FirstOrDefault(Item => Item.StationName == ActiveStation);

            if (ThisStation != null)
            {
                ThreadComboClearItemHandler(DeviceCombo);
                List<DEVICEMAPPER> DeviceList = ThisStation.GetAvaliableDevices();

                foreach (DEVICEMAPPER ExistDevice in DeviceList)
                    ThreadComboAddItemHandler(DeviceCombo, ExistDevice.DeviceName.ToString());

                ThreadComboSelectItemHandler(DeviceCombo, 0);
            }
        }

        public static void SetActiveDevice(String ActiveDeviceNameStr)
        {
            UIHandler.ActiveDeviceNameStr = ActiveDeviceNameStr;
            DeviceGrid.Rows.Clear();

            StationHandler ThisStation = AstroData.KeeperData.FirstOrDefault(Item => Item.StationName == ActiveStation);
            DEVICENAME ThisDevice = Enum.GetValues(typeof(DEVICENAME)).Cast<DEVICENAME>().ToList().FirstOrDefault(Item => Item.ToString() == ActiveDeviceNameStr);
            DEVICECATEGORY DeviceName = ThisStation.GetDeviceCategoryByDeviceName(ThisDevice);

            switch (DeviceName)
            {
                case DEVICECATEGORY.TS700MM:
                    ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT> TS700MMObject = (ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in TS700MMObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.ASTROHEVENDOME:
                    ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT> ASTROHEVENDOMEObject = (ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ASTROHEVENDOMEObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.IMAGING:
                    ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT> IMAGINGObject = (ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in IMAGINGObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.SQM:
                    ConcurrentDictionary<SQM, INFORMATIONSTRUCT> SQMObject = (ConcurrentDictionary<SQM, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in SQMObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.SEEING:
                    ConcurrentDictionary<SEEING, INFORMATIONSTRUCT> SEEINGObject = (ConcurrentDictionary<SEEING, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in SEEINGObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.ALLSKY:
                    ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT> ALLSKYObject = (ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ALLSKYObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.WEATHERSTATION:
                    ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT> WEATHERSTATIONObject = (ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in WEATHERSTATIONObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.LANOUTLET:
                    ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT> LANOUTLETObject = (ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in LANOUTLETObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.CCTV:
                    ConcurrentDictionary<CCTV, INFORMATIONSTRUCT> CCTVObject = (ConcurrentDictionary<CCTV, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in CCTVObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.GPS:
                    ConcurrentDictionary<GPS, INFORMATIONSTRUCT> GPSObject = (ConcurrentDictionary<GPS, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in GPSObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.ASTROCLIENT:
                    ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ASTROCLIENTObject = (ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ASTROCLIENTObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
                case DEVICECATEGORY.ASTROSERVER:
                    ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT> ASTROSERVERObject = (ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT>)ThisStation.DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == ThisDevice).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ASTROSERVERObject.Values)
                        DisplayAllField(ActiveStation, ActiveDeviceNameStr, ThisInformation.FieldName, ThisInformation);
                    break;
            }

            //if (OnlineDevice == null)
            //    return;

            //String ThisDeviceStatus = OnlineDevice.FirstOrDefault(Item => Item == ActiveDeviceNameStr);
            //if (ThisDeviceStatus != null && ThisStation.IsStationConnected)
            //{
            //    DeviceStatus.ForeColor = System.Drawing.Color.Green;
            //    DeviceStatus.Text = "Online";
            //}
            //else
            //{
            //    DeviceStatus.ForeColor = System.Drawing.Color.Red;
            //    DeviceStatus.Text = "Offline";
            //}
        }

        private static void DisplayAllField(STATIONNAME StationName, String DeviceNameStr, dynamic FieldName, INFORMATIONSTRUCT ThisInformation)
        {
            String FieldNameStr = FieldName.ToString();

            if (ActiveStation == StationName && FieldNameStr != "NULL")
                ThreadAddDataGridHandler(FieldNameStr, ThisInformation.Value == null ? "-" : ThisInformation.Value.ToString(), ThisInformation.UpdateTime == null ? "-" : ThisInformation.UpdateTime.ToString());
        }

        public static void DisplayToUI(STATIONNAME StationName, DEVICENAME DeviceNameStr, INFORMATIONSTRUCT ThisInformation)
        {
            if (ActiveStation == StationName && ActiveDeviceNameStr == DeviceNameStr.ToString())
            {
                Boolean IsFound = false;
                ThreadTextHandler(StationStatus, "Online");
                ThreadTextHandler(StationLastestTimeUpdate, ThisInformation.UpdateTime.Value.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                for (int i = 0; i < DeviceGrid.RowCount; i++)
                {
                    if (DeviceGrid[1, i].Value.ToString() == ThisInformation.FieldName.ToString())
                    {
                        DeviceGrid[2, i].Value = ThisInformation.Value.ToString().Length > 30 ? "Too large information to display." : ThisInformation.Value;
                        DeviceGrid[3, i].Value = ThisInformation.UpdateTime;

                        IsFound = true;
                        ThreadTextHandler(DeviceStatus, "Online");
                        break;
                    }
                }

                if (!IsFound)
                    ThreadAddDataGridHandler(ThisInformation.FieldName.ToString(), ThisInformation.Value.ToString(), ThisInformation.UpdateTime.ToString());
            }
            else if (ActiveStation == StationName && DeviceCombo.Items.Count == 0)
                SetDeviceList();
        }

        public static DEVICECATEGORY? GetDeviceENUM(String DeviceNameStr)
        {
            Array DeviceNameList = Enum.GetValues(typeof(DEVICECATEGORY));

            foreach (DEVICECATEGORY ThisDeviceName in DeviceNameList)
                if (ThisDeviceName.ToString() == DeviceNameStr)
                    return ThisDeviceName;

            return null;
        }

        public static void StationLostConnectionHandler()
        {
            ThreadTextHandler(StationStatus, "Offline");
            ThreadTextHandler(DeviceStatus, "Offline");
        }

        //------------------------------------------------------------Thread Handler----------------------------------------------------------------

        private static void ThreadComboSelectItemHandler(ComboBox Controller, int SelectedIndex)
        {
            try
            {
                if (Controller.InvokeRequired)
                    Controller.Invoke(new MethodInvoker(delegate { Controller.SelectedIndex = SelectedIndex; }));
                else
                    Controller.SelectedIndex = SelectedIndex;
            }
            catch { }
        }

        private static void ThreadComboClearItemHandler(ComboBox Controller)
        {
            if (Controller.InvokeRequired)
                Controller.Invoke(new MethodInvoker(delegate { Controller.Items.Clear(); }));
            else
                Controller.Items.Clear();
        }

        private static void ThreadComboAddItemHandler(ComboBox Controller, String Item)
        {
            if (Controller.InvokeRequired)
                Controller.Invoke(new MethodInvoker(delegate
                {
                    for (int i = 0; i < Controller.Items.Count; i++)
                        if (Controller.Items[i].ToString() == Item)
                            return;

                    Controller.Items.Add(Item);
                }));
            else
            {
                for (int i = 0; i < Controller.Items.Count; i++)
                    if (Controller.Items[i].ToString() == Item)
                        return;

                Controller.Items.Add(Item);
            }
        }

        private static void ThreadTextHandler(Control Controller, String Message)
        {
            if (Controller.InvokeRequired)
                Controller.Invoke(new MethodInvoker(delegate
                {
                    Controller.Text = Message;

                    if (Message == "Online")
                        Controller.ForeColor = Color.Green;
                    else if (Message == "Offline")
                        Controller.ForeColor = Color.Red;
                }));
            else
            {
                Controller.Text = Message;

                if (Message == "Online")
                    Controller.ForeColor = Color.Green;
                else if (Message == "Offline")
                    Controller.ForeColor = Color.Red;
            }
        }

        private static void ThreadAddDataGridHandler(String CommandName, String Value, String UpdateTime)
        {
            try
            {
                if (DeviceGrid.InvokeRequired)
                    DeviceGrid.Invoke(new MethodInvoker(delegate { DeviceGrid.Rows.Add(DeviceGrid.RowCount + 1, CommandName, Value, UpdateTime); }));
                else
                    DeviceGrid.Rows.Add(DeviceGrid.RowCount + 1, CommandName, Value, UpdateTime);
            }
            catch { }
        }
    }
}
