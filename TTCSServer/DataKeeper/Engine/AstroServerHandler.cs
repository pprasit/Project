using DataKeeper.Engine.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataKeeper.Engine
{
    public class AstroServerHandler
    {
        public Boolean IsStopTTCSLoop = false;
        private DateTime? TTCSStartTime = null;

        public void StartAstroServer()
        {
            GetServerStartTime();
            CreateAstroStation();

            Task ServerTask = Task.Run(async () =>
            {
                while (true)
                {
                    GetServerUpTime();
                    GetOnlineStation();
                    GetOnlineDevice();
                    GetAvaliableDevice();
                    
                    await Task.Delay(1000);
                }
            });

            Task MissingTask = Task.Run(async () =>
            {
                while (true)
                {
                    GetMissingData();
                    await Task.Delay(5000);
                }
            });
        }

        private void GetServerStartTime()
        {
            TTCSStartTime = DateTime.Now;
        }

        private void CreateAstroStation()
        {
            ReturnKnowType ReturnResult = AstroData.CreateStation(STATIONNAME.ASTROSERVER, "ASTROSERVER", this);

            if (ReturnResult.ReturnType == ReturnStatus.FAILED)
                IsStopTTCSLoop = true;
        }

        private void GetAvaliableDevice()
        {
            AstroData.NewASTROSERVERInformation(STATIONNAME.ASTROSERVER, DEVICENAME.ASTROPARK_SERVER, ASTROSERVER.ASTROSERVER_AVALIABLEDEVICES, DEVICECATEGORY.ASTROSERVER.ToString(), DateTime.Now);
        }

        private void GetServerUpTime()
        {
            if (TTCSStartTime == null)
                return;

            TimeSpan Span = DateTime.Now - TTCSStartTime.Value;
            AstroData.NewASTROSERVERInformation(STATIONNAME.ASTROSERVER,  DEVICENAME.ASTROPARK_SERVER, ASTROSERVER.ASTROSERVER_UPTIME_DATA, Span.ToString(@"dd\.hh\:mm\:ss"), DateTime.Now);
        }

        private void GetOnlineStation()
        {
            ReturnKnowType ThisResult = AstroData.GetAllOnlineStation();
            if (ThisResult.ReturnValue != null)
            {
                List<StationHandler> AllStation = (List<StationHandler>)ThisResult.ReturnValue;

                List<String> OnlineStation = new List<string>();
                foreach (StationHandler ThisStation in AllStation)
                {
                    if (ThisStation.IsStationConnected)
                        OnlineStation.Add(ThisStation.StationName.ToString());
                }

                if (OnlineStation.Count > 0)
                    AstroData.NewASTROSERVERInformation(STATIONNAME.ASTROSERVER, DEVICENAME.ASTROPARK_SERVER, ASTROSERVER.ASTROSERVER_ALLONLINESTATION, String.Join(", ", OnlineStation), DateTime.Now);
            }
            else
                AstroData.NewASTROSERVERInformation(STATIONNAME.ASTROSERVER, DEVICENAME.ASTROPARK_SERVER, ASTROSERVER.ASTROSERVER_ONLINEDEVICES, "", DateTime.Now);
        }

        private void GetMissingData()
        {
            ReturnKnowType ThisResult = AstroData.GetAllOnlineStation();
            if (ThisResult.ReturnValue != null)
            {
                List<StationHandler> AllStation = (List<StationHandler>)ThisResult.ReturnValue;

                foreach (StationHandler ThisStation in AllStation)
                {
                    if (ThisStation.IsStationConnected && ThisStation.StationName != STATIONNAME.ASTROSERVER)
                    {
                        StationHandler StationCommunication = AstroData.GetStationObject(ThisStation.StationName);
                        StationCommunication.CheckLastesInformation(DateTime.UtcNow.Ticks, out String Message);
                    }
                }
            }
        }

        private void GetOnlineDevice()
        {
            //ReturnKnowType ThisResult = TTCSData.GetOnlineStation(STATIONNAME.ASTROSERVER);

            //if (ThisResult.ReturnValue != null)
            //{
            //    StationHandler ThisStation = (StationHandler)ThisResult.ReturnValue;

            //    List<String> OnlineDevice = new List<String>();
            //    foreach (T07DeviceStructure ThisDevice in ThisStation.DeviceStroage.Values)
            //        OnlineDevice.Add(ThisDevice.DeviceName.ToString());

            //    if (OnlineDevice.Count > 0)
            //        TTCSData.NewData(STATIONNAME.MainServer, DEVICENAME.TTCSMainServer,Command.InformationdName.TTCSSERVER_ONLINEDEVICES, String.Join(", ", OnlineDevice), DateTime.Now);
            //}
            //else
            //    TTCSData.NewData(STATIONNAME.MainServer, DEVICENAME.TTCSMainServer, Command.InformationdName.TTCSSERVER_ONLINEDEVICES, "", DateTime.Now);
        }
    }
}
