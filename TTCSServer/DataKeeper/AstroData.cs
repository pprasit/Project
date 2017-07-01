using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataKeeper.Engine;
using System.Reflection;
using System.Threading;
using DataKeeper.Engine.Command;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Runtime.Remoting;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing;

namespace DataKeeper
{
    public class InformationBuffering
    {
        public STATIONNAME StationName { get; set; }
        public DEVICECATEGORY DeviceName { get; set; }
        public String CommandName { get; set; }
        public Object Value { get; set; }
        public DateTime DataTimestamp { get; set; }
    }

    public static class AstroData
    {
        public static List<StationHandler> KeeperData = null;
        private static List<InformationBuffering> InfoBuffer = new List<InformationBuffering>();
        private static Object MainWindows = null;

        public static void CreateTTCSData(Object MainWindows)
        {
            AstroData.MainWindows = MainWindows;
            KeeperData = new List<StationHandler>();
        }

        public static List<DEVICEMAPPER> GetAvaliableDevice(STATIONNAME StationName)
        {
            if (KeeperData != null)
            {
                StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
                if (ThisSite != null)
                    return ThisSite.GetAvaliableDevices();
            }
            return null;
        }

        public static DEVICECATEGORY GetDeviceCategoryByDeviceName(STATIONNAME StationName, DEVICENAME DeviceName)
        {
            if (KeeperData != null)
            {
                StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
                if (ThisSite != null)
                    return ThisSite.GetDeviceCategoryByDeviceName(DeviceName);
            }
            return DEVICECATEGORY.NULL;
        }

        public static StationHandler GetStationObject(STATIONNAME StationName)
        {
            if (KeeperData != null)
                return KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            return null;
        }

        public static ReturnKnowType GetOnlineStation(STATIONNAME StationName)
        {
            if (KeeperData != null)
            {
                StationHandler ThisStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

                if (ThisStation != null)
                    return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null, ThisStation);
                else
                    return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#TT015) Failed to get online station because all station " + StationName + " is offline.");
            }
            else
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#TT016) Failed to get online station because all station " + StationName + " is offline.");
        }

        public static ReturnKnowType GetAllOnlineStation()
        {
            if (KeeperData != null)
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null, KeeperData);
            else
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#TT017) Failed to get online station because all station is offline.");
        }

        public static ReturnKnowType CreateStation(STATIONNAME StationName, String StationSessionID, Object ServerCallBackObject)
        {
            StationHandler ThisStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            ReturnKnowType CreateSiteResult = ThisStation.CreateEngine(StationSessionID, ServerCallBackObject);

            if (ThisStation != null)
                ThisStation.StationConnected();

            TTCSLog.NewLogInformation(StationName, DateTime.Now, "Station name : " + StationName.ToString() + " has been created, Status ready and waiting for command.", LogType.COMMUNICATION, null);
            return CreateSiteResult;
        }

        public static void CreateStation(STATIONNAME StationName, List<DEVICEMAPPER> AvaliableDevices)
        {
            StationHandler NewStation = new StationHandler();
            NewStation.CreateEngine(StationName, AvaliableDevices);

            KeeperData.Add(NewStation);
        }

        public static void StationDisconnected(STATIONNAME StationName)
        {
            StationHandler ThisStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            ThisStation.StationDisconnected();
        }

        public static STATIONNAME GetStationName(String StationSessionID)
        {
            StationHandler ThisStation = KeeperData.FirstOrDefault(Item => Item.StationSessionID == StationSessionID);

            if (ThisStation != null)
                return ThisStation.StationName;
            else
                return STATIONNAME.NULL;
        }

        public static Boolean IsStationConnected(String StationSessionID)
        {
            StationHandler ThisStation = KeeperData.FirstOrDefault(Item => Item.StationSessionID == StationSessionID);

            if (ThisStation != null)
                return ThisStation.IsStationConnected;
            else
                return false;
        }

        public static void SetStationDisconnected(String StationSessionID)
        {
            StationHandler ThisStation = KeeperData.FirstOrDefault(Item => Item.StationSessionID == StationSessionID && Item.IsStationConnected);

            if (ThisStation != null)
                ThisStation.IsStationConnected = false;

            UIHandler.StationLostConnectionHandler();
        }

        public static void NewTS700MMInformation(STATIONNAME StationName, DEVICENAME DeviceName, TS700MM FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewTS700MMInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewASTROHEVENDOMEInformation(STATIONNAME StationName, DEVICENAME DeviceName, ASTROHEVENDOME FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewASTROHEVENDOMEInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewIMAGINGInformation(STATIONNAME StationName, DEVICENAME DeviceName, IMAGING FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
            {
                ExistingStation.NewIMAGINGInformation(DeviceName, FieldName, Value, DataTimestamp);

                switch (FieldName)
                {
                    case IMAGING.IMAGING_CCD_DOWNLOAD_STATUS: if (Value.ToString() == "Completed") LoadFITSImage(StationName, DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_PREVIEW_DOWNLOAD_STATUS: if (Value.ToString() == "Completed") LoadPerviewImage(StationName, DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_STARTX: SubFrameChecking(DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_STARTY: SubFrameChecking(DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_NUMX: SubFrameChecking(DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_NUMY: SubFrameChecking(DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_CCD_BINX: SubFrameChecking(DeviceName, ExistingStation); break;
                    case IMAGING.IMAGING_CCD_BINY: SubFrameChecking(DeviceName, ExistingStation); break;
                }
            }
        }

        private static void SubFrameChecking(DEVICENAME DeviceName, StationHandler ExistingStation)
        {
            try
            {
                int STARTX = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_STARTX).Value);
                int STARTY = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_STARTX).Value);
                int CAMERAXSIZE = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_CCD_CAMERAXSIZE).Value);
                int CAMERAYSIZE = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_CCD_CAMERAYSIZE).Value);
                int NUMX = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_STARTX).Value);
                int NUMY = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_STARTX).Value);
                int BINX = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_CCD_BINX).Value);
                int BINY = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_CCD_BINY).Value);

                if (STARTX > 0 && STARTY > 0 && NUMX < CAMERAXSIZE && NUMY < CAMERAYSIZE && BINX > 1 && BINY > 1)
                    ExistingStation.NewIMAGINGInformation(DeviceName, IMAGING.IMAGING_CCD_ISSUBFRAMEON, true, DateTime.Now);
                else
                    ExistingStation.NewIMAGINGInformation(DeviceName, IMAGING.IMAGING_CCD_ISSUBFRAMEON, false, DateTime.Now);
            }
            catch
            {
                ExistingStation.NewIMAGINGInformation(DeviceName, IMAGING.IMAGING_CCD_ISSUBFRAMEON, false, DateTime.Now);
            }
        }

        private static void LoadPerviewImage(STATIONNAME StationName, DEVICENAME DeviceName, StationHandler ExistingStation)
        {
            String GetPath = "";
            switch (StationName)
            {
                case STATIONNAME.AIRFORCE: GetPath = @"c:\FTP\AF\PreviewImg.jpg"; break;
                case STATIONNAME.ASTROPARK: GetPath = @"c:\FTP\ASP\PreviewImg.jpg"; break;
                case STATIONNAME.CHACHOENGSAO: GetPath = @"c:\FTP\CCO\PreviewImg.jpg"; break;
                case STATIONNAME.NAKHONRATCHASIMA: GetPath = @"c:\FTP\NKA\PreviewImg.jpg"; break;
                case STATIONNAME.CHINA: GetPath = @"c:\FTP\CHA\PreviewImg.jpg"; break;
                case STATIONNAME.AUSTRIA: GetPath = @"c:\FTP\AUS\PreviewImg.jpg"; break;
                case STATIONNAME.USA: GetPath = @"c:\FTP\USA\PreviewImg.jpg"; break;
            }

            Task DomeTask = Task.Run(() =>
            {
                using (Image image = Image.FromFile(GetPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        ExistingStation.NewIMAGINGInformation(DeviceName, IMAGING.IMAGING_PREVIEW_BASE64, imageBytes, DateTime.Now);
                        ExistingStation.NewIMAGINGInformation(DeviceName, IMAGING.IMAGING_PREVIEW_READY, true, DateTime.Now);
                    }
                }
            });
        }

        private static void LoadFITSImage(STATIONNAME StationName, DEVICENAME DeviceName, StationHandler ExistingStation)
        {
            Image<Gray, UInt16> ImageData = null;
            String PathName = null;

            switch (StationName)
            {
                case STATIONNAME.AIRFORCE: PathName = @"c:\FTP\AF\Lastest.FITS"; break;
                case STATIONNAME.ASTROPARK: PathName = @"c:\FTP\ASP\Lastest.FITS"; break;
                case STATIONNAME.CHACHOENGSAO: PathName = @"c:\FTP\CCO\Lastest.FITS"; break;
                case STATIONNAME.NAKHONRATCHASIMA: PathName = @"c:\FTP\NKA\Lastest.FITS"; break;
                case STATIONNAME.CHINA: PathName = @"c:\FTP\CHA\Lastest.FITS"; break;
                case STATIONNAME.AUSTRIA: PathName = @"c:\FTP\AUS\Lastest.FITS"; break;
                case STATIONNAME.USA: PathName = @"c:\FTP\USA\Lastest.FITS"; break;
            }

            if (PathName != null)
            {
                Task ImagingTask = Task.Run(() =>
                {
                    //int Ratio = Convert.ToInt32(ExistingStation.GetInformation(DeviceName, IMAGING.IMAGING_PREVIEW_RATIO).Value);

                    ImageData = FITSHandler.ReadFITSFile(PathName);

                    if (ImageData != null)
                        ExistingStation.NewIMAGINGInformation(DeviceName, IMAGING.IMAGING_CCD_IMAGEARRAY16, ImageData.Data, DateTime.Now);

                    //ImageData = FITSHandler.ResizeImage(ImageData, Width, Height);
                    //Double MinIntensity = 0;
                    //Double MaxIntensity = 0;
                    //FITSHandler.GetPercentileProfile(out MinIntensity, out MaxIntensity);
                    //FITSHandler.StretchImage(ImageData, );

                    //FITSHandler.SaveImage(ImageData, StationName.ToString());
                });
            }
        }

        public static void NewSQMInformation(STATIONNAME StationName, DEVICENAME DeviceName, SQM FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewSQMInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewSEEINGInformation(STATIONNAME StationName, DEVICENAME DeviceName, SEEING FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewSEEINGInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewALLSKYInformation(STATIONNAME StationName, DEVICENAME DeviceName, ALLSKY FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewALLSKYInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewWEATHERSTATIONInformation(STATIONNAME StationName, DEVICENAME DeviceName, WEATHERSTATION FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewWEATHERSTATIONInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewLANOUTLETInformation(STATIONNAME StationName, DEVICENAME DeviceName, LANOUTLET FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewLANOUTLETInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewCCTVInformation(STATIONNAME StationName, DEVICENAME DeviceName, CCTV FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewCCTVInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewGPSInformation(STATIONNAME StationName, DEVICENAME DeviceName, GPS FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewGPSInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewASTROCLIENTInformation(STATIONNAME StationName, DEVICENAME DeviceName, ASTROCLIENT FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (ExistingStation != null)
                ExistingStation.NewASTROCLIENTInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static void NewASTROSERVERInformation(STATIONNAME StationName, DEVICENAME DeviceName, ASTROSERVER FieldName, Object Value, DateTime DataTimestamp)
        {
            StationHandler ExistingStation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ExistingStation != null)
                ExistingStation.NewASTROSERVERInformation(DeviceName, FieldName, Value, DataTimestamp);
        }

        public static ReturnKnowType ScriptHandler(STATIONNAME StationDestination, ScriptStructure[] ThisScriptList)
        {
            StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == StationDestination);

            if (ThisSite != null)
            {
                Boolean AckState = false;

                if (StationDestination == STATIONNAME.ASTROSERVER)
                    AckState = true;
                else
                    return ThisSite.SendScriptToStation(ThisScriptList);

                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null, AckState);
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#TT003) Failed to relay set command to station.", false);
        }

        public static ReturnKnowType SetCommandHandler(STATIONNAME StationName, DEVICECATEGORY DeviceCategory, DEVICENAME DeviceName, dynamic CommandName, Object[] Values, DateTime CommandDateTime)
        {
            StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (ThisSite != null)
            {
                Boolean AckState = false;

                if (StationName == STATIONNAME.ASTROSERVER)
                    AckState = true;
                else
                    return ThisSite.RelayCommandToStation(DeviceCategory, DeviceName, CommandName, Values);

                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null, AckState);
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#TT003) Failed to relay set command to station.", false);
        }

        public static ReturnKnowType UpdateStationUser(String UserID, String UserName, String UserLoginName, String UserLoginPassword, String UserPermissionType, String USerStationPermission,String StationName, DATAACTION UserAction)
        {
            STATIONNAME ThisStationName = TTCSHelper.StationStrConveter(StationName);
            StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == ThisStationName);

            if (ThisSite != null)
            {
                ReturnKnowType ThisResult = ThisSite.UpdateStationUser(UserID, UserName, UserLoginName, UserLoginPassword, UserPermissionType, USerStationPermission, UserAction);
                if (ThisResult.ReturnType == ReturnStatus.SUCESSFUL)
                    return ThisResult;
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#TT004) Can not update user data to station. Will try again.", false);
        }

        public static ReturnKnowType GRBHandler(String Ra, String Dec, String FOV, DateTime UpdateTIme)
        {
            StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == STATIONNAME.TNO);

            if (ThisSite == null)
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Station TNO 2.4 Meter Telescope is now offline. Please contact Pakawat Prasit", null);

            try
            {
                //ReturnDeviceInformation OutHum = ThisSite.GetDataT07ByCommandName(DEVICENAME.T24WSC, InformationdName.WEATHER_ATTRIBUTE_DATA_OUTHUM);
                //ReturnDeviceInformation Temp = ThisSite.GetDataT07ByCommandName(DEVICENAME.T24WSC, InformationdName.WEATHER_ATTRIBUTE_DATA_TEMPOUT);
                //ReturnDeviceInformation DewPt = ThisSite.GetDataT07ByCommandName(DEVICENAME.T24WSC, InformationdName.WEATHER_ATTRIBUTE_DATA_DEWPT);
                //ReturnDeviceInformation InfoDate = ThisSite.GetDataT07ByCommandName(DEVICENAME.T24WSC, InformationdName.WEATHER_ATTRIBUTE_DATA_DATE);
                //ReturnDeviceInformation InfoTime = ThisSite.GetDataT07ByCommandName(DEVICENAME.T24WSC, InformationdName.WEATHER_ATTRIBUTE_DATA_TIME);

                //if (Convert.ToDouble(OutHum.ReturnValue.Value) > 85)
                //    return ReturnKnowType.DefineReturn(ReturnStatus.Failed, "We can not open the dome because the humidity is heighter than 85%. (Weather time is " + InfoDate.ReturnValue.Value.ToString() +
                //        " " + InfoTime.ReturnValue.Value.ToString() + ")", null);
                //else if (Convert.ToDouble(Temp.ReturnValue.Value) - Convert.ToDouble(DewPt.ReturnValue.Value) <= 2)
                //    return ReturnKnowType.DefineReturn(ReturnStatus.Failed, "We can not open the dome because the dew point is too height. (Weather time is " + InfoDate.ReturnValue.Value.ToString() +
                //        " " + InfoTime.ReturnValue.Value.ToString() + ")", null);
            }
            catch { }

            return ThisSite.RelayGRB(Ra, Dec, FOV, UpdateTIme);
        }

        #region Get Information

        public static List<OUTPUTSTRUCT> GetInformation(STATIONNAME StationName)
        {
            StationHandler SiteInformation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (SiteInformation != null)
                return SiteInformation.GetInformation();
            return null;
        }

        public static List<OUTPUTSTRUCT> GetInformation(STATIONNAME StationName, DEVICECATEGORY DeviceCategory)
        {
            StationHandler SiteInformation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (SiteInformation != null)
            {
                List<OUTPUTSTRUCT> ThisOutput = SiteInformation.GetInformation(DeviceCategory);
                return ThisOutput;
            }
            return null;
        }

        public static OUTPUTSTRUCT GetInformation(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName)
        {
            StationHandler SiteInformation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (SiteInformation != null)
                return SiteInformation.GetInformation(DeviceName, FieldName);
            return null;
        }

        public static OUTPUTSTRUCT GetInformation(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName, Object[] Paramter)
        {
            StationHandler SiteInformation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (SiteInformation != null)
                return SiteInformation.GetInformation(DeviceName, FieldName);
            return null;
        }

        public static INFORMATIONSTRUCT GetInformationObject(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName)
        {
            StationHandler SiteInformation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (SiteInformation != null)
                return SiteInformation.GetInformationObject(DeviceName, FieldName);
            return null;
        }

        #endregion

        #region Subscribe Information

        public static void SubscribeInformation(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName, String SessionID, Object CallBackObject)
        {
            StationHandler SiteInformation = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);

            if (SiteInformation != null)
                SiteInformation.SubscribeInformation(DeviceName, FieldName, SessionID, CallBackObject);
        }

        #endregion

        #region Unsubscribe Information

        public static void UnsubscribeBySessionID(String SessionID)
        {
            foreach (StationHandler ThisStation in KeeperData)
            {
                if (ThisStation != null)
                    ThisStation.UnsubscribeBySessionID(SessionID);
            }
        }

        public static void UnsubscribeByFieldName(String SessionID, STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName)
        {
            StationHandler ThisSite = KeeperData.FirstOrDefault(Item => Item.StationName == StationName);
            if (ThisSite != null)
                ThisSite.UnsubscribeByFieldName(SessionID, DeviceName, FieldName);
        }

        #endregion

        public static STATIONNAME? SiteConveter(String Name)
        {
            if (Name == STATIONNAME.ASTROSERVER.ToString())
                return STATIONNAME.ASTROSERVER;
            else if (Name == STATIONNAME.AUSTRIA.ToString())
                return STATIONNAME.AUSTRIA;
            else if (Name == STATIONNAME.CHINA.ToString())
                return STATIONNAME.CHINA;
            else if (Name == STATIONNAME.AIRFORCE.ToString())
                return STATIONNAME.AIRFORCE;
            else if (Name == STATIONNAME.SONGKLA.ToString())
                return STATIONNAME.SONGKLA;
            else if (Name == STATIONNAME.USA.ToString())
                return STATIONNAME.USA;
            else if (Name == STATIONNAME.TNO.ToString())
                return STATIONNAME.TNO;
            else
                return null;
        }

        public static DEVICECATEGORY? DeviceConverter(String Name)
        {
            if (Name == DEVICECATEGORY.TS700MM.ToString())
                return DEVICECATEGORY.TS700MM;
            else if (Name == DEVICECATEGORY.ASTROHEVENDOME.ToString())
                return DEVICECATEGORY.ASTROHEVENDOME;
            else if (Name == DEVICECATEGORY.WEATHERSTATION.ToString())
                return DEVICECATEGORY.WEATHERSTATION;
            else if (Name == DEVICECATEGORY.CCTV.ToString())
                return DEVICECATEGORY.CCTV;
            else if (Name == DEVICECATEGORY.IMAGING.ToString())
                return DEVICECATEGORY.IMAGING;
            else if (Name == DEVICECATEGORY.LANOUTLET.ToString())
                return DEVICECATEGORY.LANOUTLET;
            else if (Name == DEVICECATEGORY.GPS.ToString())
                return DEVICECATEGORY.GPS;
            else if (Name == DEVICECATEGORY.SQM.ToString())
                return DEVICECATEGORY.SQM;
            else if (Name == DEVICECATEGORY.SEEING.ToString())
                return DEVICECATEGORY.SEEING;
            else if (Name == DEVICECATEGORY.ALLSKY.ToString())
                return DEVICECATEGORY.ALLSKY;
            else if (Name == DEVICECATEGORY.CLOUDSENSOR.ToString())
                return DEVICECATEGORY.CLOUDSENSOR;
            else if (Name == DEVICECATEGORY.ASTROCLIENT.ToString())
                return DEVICECATEGORY.ASTROCLIENT;

            else if (Name == DEVICECATEGORY.T244K.ToString())
                return DEVICECATEGORY.T244K;
            else if (Name == DEVICECATEGORY.T24CAN.ToString())
                return DEVICECATEGORY.T24CAN;
            else if (Name == DEVICECATEGORY.T24DIO.ToString())
                return DEVICECATEGORY.T24DIO;
            else if (Name == DEVICECATEGORY.T24DIS.ToString())
                return DEVICECATEGORY.T24DIS;
            else if (Name == DEVICECATEGORY.T24DOME.ToString())
                return DEVICECATEGORY.T24DOME;
            else if (Name == DEVICECATEGORY.T24GPS.ToString())
                return DEVICECATEGORY.T24GPS;
            else if (Name == DEVICECATEGORY.T24MRES.ToString())
                return DEVICECATEGORY.T24MRES;
            else if (Name == DEVICECATEGORY.T24SI.ToString())
                return DEVICECATEGORY.T24SI;
            else if (Name == DEVICECATEGORY.T24TEMP.ToString())
                return DEVICECATEGORY.T24TEMP;
            else if (Name == DEVICECATEGORY.T24TS.ToString())
                return DEVICECATEGORY.T24TS;
            else if (Name == DEVICECATEGORY.T24WSC.ToString())
                return DEVICECATEGORY.T24WSC;
            else
                return null;
        }
    }
}
