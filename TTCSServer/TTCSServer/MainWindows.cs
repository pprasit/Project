using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TTCSServer.Interface;
using DataKeeper;
using DataKeeper.Engine;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using TTCSConnection;
using DataKeeper.Engine.Command;
using DataKeeper.Interface;

namespace TTCSServer
{
    public partial class MainWindows : Form
    {
        private ServiceHost TCPService;
        private PackageMonitoring ObjPackageMonitoring = null;
        private ScriptMonitoring ObjScriptMonitoring = null;
        private UserMonitoring ObjUserMonitoring = null;

        public MainWindows()
        {
            DBScheduleEngine.ConnectDB();
            InitializeComponent();
            InitializeInterface();
            InitializeServer();
            InitializeWebService();
            InitializeSetCommand();
            InitializeWS();            
        }

        private void InitializeSetCommand()
        {
            CommandDefinition.DefineCommand();
        }

        private void InitializeInterface()
        {
            TTCSLog.CreateTTCSLog(TTCSLogGrid);

            StationSelection.SelectedIndex = 0;
            SearchStationName.SelectedIndex = 0;
        }

        private void InitializeWS()
        {
            ObjPackageMonitoring = new PackageMonitoring();
            WebSockets.CreateConnection(ClientGrid, this, TTCSLogGrid, Properties.Settings.Default.SocketServerAddress);
        }

        private void InitializeServer()
        {
            AstroData.CreateTTCSData(this);
            TCPService = new ServiceHost(typeof(TTCSConnection.Connection));
            TCPService.Open();

            InitializeStation();

            AstroServerHandler AstroHandler = new AstroServerHandler();
            AstroHandler.StartAstroServer();

            DatabaseSynchronization.SetDBConnection(Properties.Settings.Default.DatabaseName, Properties.Settings.Default.DatabaseUserName, Properties.Settings.Default.DatabasePassword, Properties.Settings.Default.DatabaseServerName);
            FITSHandler.CreateMaximObject();
            ScriptManager.CreateScriptPool();

            ScriptEngine.NewScriptChecker("192.168.2.110", "astronet", "P@ssw0rd");
        }

        private void InitializeStation()
        {
            AstroData.CreateStation(STATIONNAME.ASTROSERVER, ASTROSERVERDeviceMap());
            AstroData.CreateStation(STATIONNAME.AIRFORCE, AIRFORCEDeviceMap());
            AstroData.CreateStation(STATIONNAME.ASTROPARK, ASTROPARKDeviceMap());
            AstroData.CreateStation(STATIONNAME.CHACHOENGSAO, CHACHOENGSAODeviceMap());
            AstroData.CreateStation(STATIONNAME.CHINA, CHINADeviceMap());
            AstroData.CreateStation(STATIONNAME.NAKHONRATCHASIMA, NAKHONRATCHASIMADeviceMap());
            AstroData.CreateStation(STATIONNAME.SONGKLA, SONGKLADeviceMap());
            AstroData.CreateStation(STATIONNAME.USA, USADeviceMap());
            AstroData.CreateStation(STATIONNAME.AUSTRALIA, AUSTRALIADeviceMap());

            AstroData.NewASTROCLIENTInformation(STATIONNAME.AIRFORCE, DEVICENAME.AIRFORCE_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.SONGKLA, DEVICENAME.SONGKLA_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.CHACHOENGSAO, DEVICENAME.CHACHOENGSAO_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.ASTROPARK, DEVICENAME.ASTROPARK_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.NAKHONRATCHASIMA, DEVICENAME.NAKHONRATCHASIMA_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.CHINA, DEVICENAME.CHINA_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.USA, DEVICENAME.USA_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            AstroData.NewASTROCLIENTInformation(STATIONNAME.AUSTRALIA, DEVICENAME.AUSTRALIA_ASTROCLIENT, ASTROCLIENT.ASTROCLIENT_LISTOFAVLIABLEDEVICES_DATA,
                GetAvaliableDevice(), DateTime.UtcNow);

            UIHandler.SetDeviceList();
        }

        private static String GetAvaliableDevice()
        {
            List<String> AllDEVICECATEGORY = new List<String>();
            AllDEVICECATEGORY.Add(DEVICECATEGORY.GPS.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.ASTROCLIENT.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.TS700MM.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.ASTROHEVENDOME.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.IMAGING.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.LANOUTLET.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.CCTV.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.WEATHERSTATION.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.ALLSKY.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.SQM.ToString());
            AllDEVICECATEGORY.Add(DEVICECATEGORY.CLOUDSENSOR.ToString());

            return String.Join(", ", AllDEVICECATEGORY.ToArray());
        }

        #region Device Mapper

        private List<DEVICEMAPPER> ASTROSERVERDeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROSERVER, DeviceName = DEVICENAME.ASTROPARK_SERVER });
            return DeviceObject;
        }

        private List<DEVICEMAPPER> AIRFORCEDeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.AIRFORCE_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.AIRFORCE_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.AIRFORCE_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.AIRFORCE_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.AIRFORCE_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.AIRFORCE_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.AIRFORCE_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.AIRFORCE_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.AIRFORCE_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.AIRFORCE_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.AIRFORCE_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> SONGKLADeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.SONGKLA_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.SONGKLA_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.SONGKLA_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.SONGKLA_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.SONGKLA_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.SONGKLA_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.SONGKLA_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.SONGKLA_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.SONGKLA_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.SONGKLA_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.SONGKLA_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> CHACHOENGSAODeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.CHACHOENGSAO_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.CHACHOENGSAO_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.CHACHOENGSAO_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.CHACHOENGSAO_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.CHACHOENGSAO_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.CHACHOENGSAO_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.CHACHOENGSAO_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.CHACHOENGSAO_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.CHACHOENGSAO_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.CHACHOENGSAO_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.CHACHOENGSAO_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> ASTROPARKDeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.ASTROPARK_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.ASTROPARK_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.ASTROPARK_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.ASTROPARK_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.ASTROPARK_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.ASTROPARK_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.ASTROPARK_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.ASTROPARK_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.ASTROPARK_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.ASTROPARK_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.ASTROPARK_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> NAKHONRATCHASIMADeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.NAKHONRATCHASIMA_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.NAKHONRATCHASIMA_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.NAKHONRATCHASIMA_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.NAKHONRATCHASIMA_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.NAKHONRATCHASIMA_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.NAKHONRATCHASIMA_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.NAKHONRATCHASIMA_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.NAKHONRATCHASIMA_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.NAKHONRATCHASIMA_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.NAKHONRATCHASIMA_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.NAKHONRATCHASIMA_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> CHINADeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.CHINA_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.CHINA_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.CHINA_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.CHINA_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.CHINA_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.CHINA_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.CHINA_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.CHINA_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.CHINA_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.CHINA_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.CHINA_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> USADeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.USA_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.USA_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.USA_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.USA_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.USA_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.USA_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.USA_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.USA_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.USA_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.USA_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.USA_ASTROCLIENT });

            return DeviceObject;
        }

        private List<DEVICEMAPPER> AUSTRALIADeviceMap()
        {
            List<DEVICEMAPPER> DeviceObject = new List<DEVICEMAPPER>();
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.TS700MM, DeviceName = DEVICENAME.AUSTRALIA_TS700MM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.IMAGING, DeviceName = DEVICENAME.AUSTRALIA_IMAGING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROHEVENDOME, DeviceName = DEVICENAME.AUSTRALIA_ASTROHEVENDOME });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SQM, DeviceName = DEVICENAME.AUSTRALIA_SQM });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.SEEING, DeviceName = DEVICENAME.AUSTRALIA_SEEING });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ALLSKY, DeviceName = DEVICENAME.AUSTRALIA_ALLSKY });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.WEATHERSTATION, DeviceName = DEVICENAME.AUSTRALIA_WEATHER });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.LANOUTLET, DeviceName = DEVICENAME.AUSTRALIA_LANOUTLET });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.CCTV, DeviceName = DEVICENAME.AUSTRALIA_CCTV });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.GPS, DeviceName = DEVICENAME.AUSTRALIA_GPS });
            DeviceObject.Add(new DEVICEMAPPER() { DeviceCategory = DEVICECATEGORY.ASTROCLIENT, DeviceName = DEVICENAME.AUSTRALIA_ASTROCLIENT });

            return DeviceObject;
        }

        #endregion

        private void InitializeWebService()
        {
            UserSessionHandler.StartService();
            WebHosting.CreateWebService();
        }

        public void RelayMessageToMonitoring(String Message)
        {
            ObjPackageMonitoring.SetNewInformation(Message);
        }

        private STATIONNAME GetStationNameForSearch(String SearchStationName)
        {
            if (SearchStationName == "TTCS Server")
                return STATIONNAME.ASTROSERVER;
            else if (SearchStationName == "2.4 Meter Telescope")
                return STATIONNAME.TNO;
            else if (SearchStationName == "0.7 Meter Telescope Airfoce")
                return STATIONNAME.AIRFORCE;
            else if (SearchStationName == "0.7 Meter Telescope Chachoengsao")
                return STATIONNAME.CHACHOENGSAO;
            else if (SearchStationName == "0.7 Meter Telescope Korat")
                return STATIONNAME.NAKHONRATCHASIMA;
            else if (SearchStationName == "0.7 Meter Telescope Songkhla")
                return STATIONNAME.SONGKLA;
            else if (SearchStationName == "0.7 Meter Telescope China")
                return STATIONNAME.CHINA;
            else if (SearchStationName == "0.7 Meter Telescope USA")
                return STATIONNAME.USA;
            else if (SearchStationName == "0.7 Meter Telescope AUSTRALIA")
                return STATIONNAME.AUSTRALIA;
            else
                return STATIONNAME.NULL;
        }

        private void ClearStation()
        {
            StationStatus.Text = "Offline";
            StationStatus.ForeColor = Color.Red;

            DeviceStatus.Text = "Offline";
            DeviceStatus.ForeColor = Color.Red;

            StationLastestTimeUpdate.Text = "-";
        }

        //-----------------------------------------------------------------------------------------Event Handler------------------------------------------------------

        private void SiteSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearStation();

            switch (StationSelection.SelectedIndex)
            {
                case 0: UIHandler.SetActiveStation(STATIONNAME.ASTROSERVER, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 1: UIHandler.SetActiveStation(STATIONNAME.TNO, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 2: UIHandler.SetActiveStation(STATIONNAME.AIRFORCE, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 3: UIHandler.SetActiveStation(STATIONNAME.CHACHOENGSAO, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 4: UIHandler.SetActiveStation(STATIONNAME.NAKHONRATCHASIMA, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 5: UIHandler.SetActiveStation(STATIONNAME.SONGKLA, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 6: UIHandler.SetActiveStation(STATIONNAME.ASTROPARK, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 7: UIHandler.SetActiveStation(STATIONNAME.CHINA, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 8: UIHandler.SetActiveStation(STATIONNAME.USA, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
                case 9: UIHandler.SetActiveStation(STATIONNAME.AUSTRALIA, DeviceGrid, SelectedDevice, DeviceStatus, StationStatus, StationLastestTimeUpdate); break;
            }
        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            SettingWindows _SettingWindows = new SettingWindows();
            _SettingWindows.ShowDialog(this);

            MessageBox.Show("Please restart an appication to take an effect.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AllStationStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (SearchStationStartDate.Value.Date > SearchStationEndDate.Value.Date)
                MessageBox.Show("Invalid search information. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                STATIONNAME StationName = GetStationNameForSearch(SearchStationName.Text);
                TTCSLog.GetLogBySearchInformarion(StationName, SearchStationStartDate.Value, SearchStationEndDate.Value, StationEnableSearch.Checked);
            }
        }

        private void AllStationEnableSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (SearchStationStartDate.Value.Date > SearchStationEndDate.Value.Date)
                MessageBox.Show("Invalid search information. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                if (StationEnableSearch.Checked)
                {
                    SearchStationName.Enabled = true;
                    SearchStationStartDate.Enabled = true;
                    SearchStationEndDate.Enabled = true;
                }
                else
                {
                    SearchStationName.Enabled = false;
                    SearchStationStartDate.Enabled = false;
                    SearchStationEndDate.Enabled = false;
                }

                STATIONNAME StationName = GetStationNameForSearch(SearchStationName.Text);
                TTCSLog.GetLogBySearchInformarion(StationName, SearchStationStartDate.Value, SearchStationEndDate.Value, StationEnableSearch.Checked);
            }
        }

        private void SearchStationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SearchStationStartDate.Value.Date > SearchStationEndDate.Value.Date)
                MessageBox.Show("Invalid search information. Please check.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                STATIONNAME StationName = GetStationNameForSearch(SearchStationName.Text);
                TTCSLog.GetLogBySearchInformarion(StationName, SearchStationStartDate.Value, SearchStationEndDate.Value, StationEnableSearch.Checked);
            }
        }

        private void SelectedDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIHandler.SetActiveDevice(SelectedDevice.Text);
        }

        private void DeviceGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridView ThisGrid = (DataGridView)sender;
                    Clipboard.SetText(ThisGrid[1, e.RowIndex].Value.ToString());
                }
            }
            catch { }
        }

        private void ClientGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                String IPAddress = ClientGrid[1, e.RowIndex].Value.ToString();
                String Port = ClientGrid[2, e.RowIndex].Value.ToString();

                List<String> ThisMessage = WebSockets.GetMonitoringInformation(IPAddress, Port);
                if (ThisMessage != null)
                {
                    ObjPackageMonitoring.SetInformation(ThisMessage);
                    ObjPackageMonitoring.ClientInformationSetter(IPAddress, Port);
                    ObjPackageMonitoring.Show();
                }
            }
        }

        private void TTCSLogGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowCount > 500)
                TTCSLogGrid.Rows.RemoveAt(0);
        }

        private void BtnScriptManager_Click(object sender, EventArgs e)
        {
            //ScriptFTPEngine.GenNewScript();

            if (ObjScriptMonitoring == null)            
                ObjScriptMonitoring = new ScriptMonitoring();                           

            ScriptEngine.SetMonitoringObject(ObjScriptMonitoring);            
            ObjScriptMonitoring.Owner = this;
            ObjScriptMonitoring.Show();

            ObjScriptMonitoring.GetScript();
        }

        private void BtnUserManagenment_Click(object sender, EventArgs e)
        {
            if (ObjUserMonitoring == null)
                ObjUserMonitoring = new UserMonitoring();

            ObjUserMonitoring.Owner = this;
            ObjUserMonitoring.Show();
        }

        private void MainWindows_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ScriptManager.IsScriptActive = false;
                Environment.Exit(0);
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LabelLocal.Text = DateTime.Now.ToString();
            LabelUTC.Text = DateTime.UtcNow.ToString();
        }
    }
}
